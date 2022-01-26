using HarmonyLib;
using SpeechMod.Unity;
using SpeechMod.Voice;
using System;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityModManagerNet;
using Extensions = SpeechMod.Unity.Extensions;

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

    public static string ChosenVoice => Settings?.AvailableVoices?[Settings.ChosenVoice]?.Split('#')[0];

    public static ISpeech Speech;

    private static string testText = "Speech Mod for Pathfinder Wrath of the Righteous";

    private static bool Load(UnityModManager.ModEntry modEntry)
    {
        Debug.Log("Speech Mod Initializing...");

        Logger = modEntry.Logger;

        if (!SetSpeech())
            return false;

        Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
        UpdateColors();

        modEntry.OnToggle = OnToggle;
        modEntry.OnGUI = OnGui;
        modEntry.OnSaveGUI = OnSaveGui;

        var harmony = new Harmony(modEntry.Info.Id);
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        Logger.Log(Speech.GetStatusMessage());

        if (!SetAvailableVoices())
            return false;

        SpeechExtensions.LoadDictionary();

        Debug.Log("Speech Mod Initialized!");

        return true;
    }

    private static bool SetAvailableVoices()
    {
        var availableVoices = Speech?.GetAvailableVoices();

        if (availableVoices == null || availableVoices.Length == 0)
        {
            Logger.Warning("No available voices found! Disabling mod!");
            return false;
        }

#if DEBUG
        Logger.Log("Available voices:");
        foreach (var voice in availableVoices)
        {
            Logger.Log(voice);
        }
#endif
        Logger.Log("Setting available voices list...");
        Settings.AvailableVoices = availableVoices.OrderBy(v => v.Split('#')[1]).ToArray();

        return true;
    }

    private static bool SetSpeech()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.OSXPlayer:
                Speech = new AppleSpeech();
                SpeechExtensions.AddUiElements<AppleVoiceUnity>(Constants.APPLE_VOICE_NAME);
                break;
            case RuntimePlatform.WindowsPlayer:
                Speech = new WindowsSpeech();
                SpeechExtensions.AddUiElements<WindowsVoiceUnity>(Constants.WINDOWS_VOICE_NAME);
                break;
            default:
                Logger.Critical($"SpeechMod is not supported on {Application.platform}!");
                return false;
        }

        return true;
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
        Settings.Rate = Speech switch
        {
            WindowsSpeech => (int)GUILayout.HorizontalSlider(Settings.Rate, -10, 10, GUILayout.Width(300f)),
            AppleSpeech => (int)GUILayout.HorizontalSlider(Settings.Rate, 150, 300, GUILayout.Width(300f)),
            _ => Settings.Rate
        };
        GUILayout.Label($" {Settings.Rate}", GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        if (Speech is WindowsSpeech)
        {
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
            GUILayout.Label("Interrupt speech on play", GUILayout.ExpandWidth(false));
            GUILayout.Space(10);
            Settings.InterruptPlaybackOnPlay = GUILayout.Toggle(Settings.InterruptPlaybackOnPlay, Settings.InterruptPlaybackOnPlay ? "Interrupt and play" : "Add to queue");
            GUILayout.EndHorizontal();
        }
        else
        {
            Settings.Volume = 100;
            Settings.Pitch = 0;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Voice", GUILayout.ExpandWidth(false));
        GUILayout.Space(10);
        Settings.ChosenVoice = GUILayout.SelectionGrid(Settings.ChosenVoice, Settings?.AvailableVoices
                .Select(v =>
                {
                    var splitV = v.Split('#');
                    return new GUIContent(splitV[0], splitV[1]);
                }).ToArray(),
                Speech is WindowsSpeech ? 3 : 5
            );
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Nationality", GUILayout.ExpandWidth(false));
        GUILayout.Space(10);
        GUILayout.Label(GUI.tooltip, GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Test selected voice", GUILayout.ExpandWidth(false));
        GUILayout.Space(10);
        testText = GUILayout.TextField(testText, GUILayout.ExpandWidth(true));
        if (GUILayout.Button("Play"))
            Speech.Speak(testText);
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

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
            SpeechExtensions.LoadDictionary();
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