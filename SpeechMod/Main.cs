﻿using HarmonyLib;
using System;
using System.Reflection;
using SpeechMod.Unity;
using SpeechMod.Voice;
using TMPro;
using UnityEngine;
using UnityModManagerNet;
using Extensions = SpeechMod.Unity.Extensions;
using Object = UnityEngine.Object;

namespace SpeechMod;

#if DEBUG
    [EnableReloading]
#endif
internal static class Main
{
    public static UnityModManager.ModEntry.ModLogger Logger;
    public static Settings Settings;
    public static bool Enabled;

    public static string[] FontStyleNames = Enum.GetNames(typeof(FontStyles));

    public static string ChosenVoice => Settings.AvailableVoices[Settings.ChosenVoice];

    private static bool Load(UnityModManager.ModEntry modEntry)
    {
        Debug.Log("Speech Mod Initializing...");

        Logger = modEntry.Logger;

        Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
        UpdateColors();

        modEntry.OnToggle = OnToggle;
        modEntry.OnGUI = OnGui;
        modEntry.OnSaveGUI = OnSaveGui;

        var harmony = new Harmony(modEntry.Info.Id);
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        Logger.Log(WindowsVoiceUnity.GetStatusMessage());

        string[] availableVoices = WindowsVoiceUnity.GetAvailableVoices();
#if DEBUG
        Logger.Log("Available voices:");
        foreach (var s in availableVoices)
        {
            Logger.Log(s);
        }
#endif
        if (availableVoices == null || availableVoices.Length == 0)
        {
            Logger.Warning("No voices available found! Diabling mod!");
            return false;
        }

        Logger.Log("Setting available voices list...");
        Settings.AvailableVoices = availableVoices;

        Speech.LoadDictionary();

        AddUiElements();

        Debug.Log("Speech Mod Initialized!");

        return true;
    }

    private static void AddUiElements()
    {
        Debug.Log("Adding SpeechMod UI elements.");

        GameObject windowsVoice = null;
        try
        {
            windowsVoice = Object.FindObjectOfType<WindowsVoiceUnity>()?.gameObject;
        }
        catch{} // Sigh

        if (windowsVoice != null)
        {
            Debug.Log($"{nameof(WindowsVoiceUnity)} found!");
            return;
        }

        Debug.Log($"Adding {nameof(WindowsVoiceUnity)}...");

        var windowsVoiceGameObject = new GameObject(Constants.WINDOWS_VOICE_NAME);
        windowsVoiceGameObject.AddComponent<WindowsVoiceUnity>();
        Object.DontDestroyOnLoad(windowsVoiceGameObject);
    }

    private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
    {
        Enabled = value;
        return true;
    }

    private static void OnGui(UnityModManager.ModEntry modEntry)
    {
        GUILayout.BeginVertical("", GUI.skin.box);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Speech rate", GUILayout.ExpandWidth(false));
        GUILayout.Space(10);
        Settings.Rate = (int)GUILayout.HorizontalSlider(Settings.Rate, -10, 10, GUILayout.Width(300f));
        GUILayout.Label($" {Settings.Rate}", GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Speech volume", GUILayout.ExpandWidth(false));
        GUILayout.Space(10);
        Settings.Volume = (int)GUILayout.HorizontalSlider(Settings.Volume, 0, 100, GUILayout.Width(300f));
        GUILayout.Label($" {Settings.Volume}", GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Speech pitch", GUILayout.ExpandWidth(false));
        Settings.Pitch = (int)GUILayout.HorizontalSlider(Settings.Pitch, -10, 10, GUILayout.Width(300f));
        GUILayout.Label($" {Settings.Pitch}", GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Voice", GUILayout.ExpandWidth(false));
        GUILayout.Space(10);
        Settings.ChosenVoice = GUILayout.SelectionGrid(Settings.ChosenVoice, Settings.AvailableVoices, 3);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Interrupt speech on play");
        GUILayout.Space(10);
        Settings.InterruptPlaybackOnPlay = GUILayout.Toggle(Settings.InterruptPlaybackOnPlay, Settings.InterruptPlaybackOnPlay ? "Interrupt and play" : "Add to queue");
        GUILayout.EndHorizontal();

        AddColorPicker("Color on text hover", ref Settings.ColorOnHover, "Hover color", ref Settings.HoverColorR, ref Settings.HoverColorG, ref Settings.HoverColorB, ref Settings.HoverColorA);

        AddColorPicker("Show playback progress", ref Settings.ShowPlaybackProgress, "Playback progress color", ref Settings.PlaybackColorR, ref Settings.PlaybackColorG, ref Settings.PlaybackColorB, ref Settings.PlaybackColorA);

        GUILayout.BeginVertical("", GUI.skin.box);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Font style on text hover", GUILayout.ExpandWidth(false));
        Settings.FontStyleOnHover = GUILayout.Toggle(Settings.FontStyleOnHover, "Enabled");
        GUILayout.EndHorizontal();

        if (Settings.FontStyleOnHover)
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < Settings.FontStyles.Length; ++i)
            {
                Settings.FontStyles[i] = GUILayout.Toggle(Settings.FontStyles[i], FontStyleNames[i], GUILayout.ExpandWidth(true));
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical("", GUI.skin.box);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Phonetic dictionary", GUILayout.ExpandWidth(false));
        GUILayout.Space(10);
        if (GUILayout.Button("Reload", GUILayout.ExpandWidth(false)))
            Speech.LoadDictionary();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private static void AddColorPicker(string enableLabel, ref bool enabledBool, string colorLabel, ref float r, ref float g, ref float b, ref float a)
    {
        GUILayout.BeginVertical("", GUI.skin.box);
        GUILayout.BeginHorizontal();
        GUILayout.Label(enableLabel, GUILayout.ExpandWidth(false));
        enabledBool = GUILayout.Toggle(enabledBool, "Enabled");
        GUILayout.EndHorizontal();

        if (enabledBool)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(colorLabel, GUILayout.ExpandWidth(false));
            GUILayout.Space(10);
            GUILayout.Label("R ", GUILayout.ExpandWidth(false));
            r = GUILayout.HorizontalSlider(r, 0, 1);
            GUILayout.Space(10);
            GUILayout.Label("G", GUILayout.ExpandWidth(false));
            g = GUILayout.HorizontalSlider(g, 0, 1);
            GUILayout.Space(10);
            GUILayout.Label("B", GUILayout.ExpandWidth(false));
            b = GUILayout.HorizontalSlider(b, 0, 1);
            GUILayout.Space(10);
            GUILayout.Label("A", GUILayout.ExpandWidth(false));
            a = GUILayout.HorizontalSlider(a, 0, 1);
            GUILayout.Space(10);
            GUILayout.Box(GetColorPreview(ref r, ref g, ref b, ref a), GUILayout.Width(20));
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    private static Texture2D GetColorPreview(ref float r, ref float g, ref float b, ref float a)
    {
        var texture = new Texture2D(20, 20);
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, new Color(r, g, b, a));
            }
        }
        texture.Apply();
        return texture;
    }

    private static void UpdateColors()
    {
        Extensions.UpdateHoverColor();
    }

    private static void OnSaveGui(UnityModManager.ModEntry modEntry)
    {
        UpdateColors();
        Settings.Save(modEntry);
    }
}