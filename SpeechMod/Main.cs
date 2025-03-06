using HarmonyLib;
using Rewired;
using SpeechMod.Configuration;
using SpeechMod.Configuration.Settings;
using SpeechMod.Keybinds;
using SpeechMod.Unity;
using SpeechMod.Voice;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityModManagerNet;

namespace SpeechMod;

#if DEBUG
[EnableReloading]
#endif
public static class Main
{
    public static UnityModManager.ModEntry.ModLogger Logger;
    public static Settings Settings;
    public static JsonSettings JsonSettings;
    public static bool Enabled;

    public static string[] FontStyleNames = Enum.GetNames(typeof(FontStyles));

    public static string NarratorVoice => VoicesDict?.ElementAtOrDefault(Settings.NarratorVoice).Key;
    public static string FemaleVoice => VoicesDict?.ElementAtOrDefault(Settings.FemaleVoice).Key;
    public static string MaleVoice => VoicesDict?.ElementAtOrDefault(Settings.MaleVoice).Key;

    public static Dictionary<string, string> VoicesDict => Settings?.AvailableVoices?.Select(v =>
    {
        var splitV = v?.Split('#');
        return splitV.Length != 2
            ? new { Key = v, Value = "Unknown" }
            : new { Key = splitV[0], Value = splitV[1] };
    }).ToDictionary(p => p.Key, p => p.Value);

    public static ISpeech Speech;
    private static bool m_Loaded = false;

    private static bool Load(UnityModManager.ModEntry modEntry)
    {
        Debug.Log("Pathfinder: Wrath of the Righteous Speech Mod Initializing...");

        Logger = modEntry.Logger;

        JsonSettings = JsonSettingsSerializer.LoadSettings(Path.Combine(modEntry.Path, "settings.json"));

        if (!SetSpeech())
            return false;

        Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
        MenuGUI.UpdateColors();

        modEntry.OnToggle = OnToggle;
        modEntry.OnGUI = OnGui;
        modEntry.OnSaveGUI = OnSaveGui;

        var harmony = new Harmony(modEntry.Info.Id);
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        ModConfigurationManager.Build(harmony, modEntry, Constants.SETTINGS_PREFIX);
        SetUpSettings();
        harmony.CreateClassProcessor(typeof(SettingsUIPatches)).Patch();

        Logger.Log(Speech.GetStatusMessage());

        if (!SetAvailableVoices())
            return false;

        PhoneticDictionary.LoadDictionary();

        // For ReInput.players.AllPlayers : 
        // 0 System, 1 MainPlayer
        if (ReInput.players.allPlayerCount >= 1)
        {
            Rewired.Player p = ReInput.players.AllPlayers[1];

            p.AddInputEventDelegate(doButtonWork, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Decline");
        }

        Debug.Log("Pathfinder: Wrath of the Righteous Speech Mod Initialized!");
        m_Loaded = true;
        return true;
    }

    public static void doButtonWork(InputActionEventData data)
    {
        // Interrupts current speech and plays the next phrase (if any)
        Speech.NextPhrase();
    }

    private static void SetUpSettings()
    {
        if (ModConfigurationManager.Instance.GroupedSettings.TryGetValue("main", out _))
            return;

        ModConfigurationManager.Instance.GroupedSettings.Add("main", [new PlaybackStop()]);
    }

    private static bool SetAvailableVoices()
    {
        var availableVoices = Speech?.GetAvailableVoices();

        if (availableVoices == null || availableVoices.Length == 0)
        {
            Logger.Warning("No available voices found! Disabling mod!");
            return false;
        }

//#if DEBUG
        Logger.Log("Available voices:");
        foreach (var voice in availableVoices)
        {
            Logger.Log(voice);
        }
//#endif
        Logger.Log("Setting available voices list...");

        for (var i = 0; i < availableVoices.Length; i++)
        {
            var splitVoice = availableVoices[i].Split('#');
            if (splitVoice.Length != 2 || string.IsNullOrEmpty(splitVoice[1]))
                availableVoices[i] = availableVoices[i].Replace("#","").Trim() + "#Unknown";
        }

        // Ensure that the selected voice index falls within the available voices range
        if (Settings.NarratorVoice >= availableVoices.Length)
        {
            Logger.Log($"{nameof(Settings.NarratorVoice)} was out of range, resetting to first voice available.");
            Settings.NarratorVoice = 0;
        }

        if (Settings.FemaleVoice >= availableVoices.Length)
        {
            Logger.Log($"{nameof(Settings.FemaleVoice)} was out of range, resetting to first voice available.");
            Settings.FemaleVoice = 0;
        }

        if (Settings.MaleVoice >= availableVoices.Length)
        {
            Logger.Log($"{nameof(Settings.MaleVoice)} was out of range, resetting to first voice available.");
            Settings.MaleVoice = 0;
        }

        Settings.AvailableVoices = availableVoices.OrderBy(v => v.Split('#').ElementAtOrDefault(1)).ToArray();

        return true;
    }

    // TODO clean up UMM configuration to better show what speech implementation is being used
    // and what can be changed in-game. I prefer the json way, anyway, so I'm not sure how much
    // I will actually change this
    private static bool SetSpeech()
    {
        // Dispose of existing speech instance if it exists
        if (Speech is IDisposable disposableSpeech)
        {
            disposableSpeech.Dispose();
        }

        // keep the setting of uielements/config section the same for now (until maybe I change it)
        // but use the json config for the speech implementation instantiation
        try {
            var className = JsonSettings.speech_impl;

            Logger.Log("Setting speech impl...." + className);

            Assembly assembly = Assembly.GetExecutingAssembly();
            Type type = assembly.GetTypes()
                .FirstOrDefault(t => t.Name.Equals(className, StringComparison.Ordinal));
            
            if (type == null)
            {
                throw new ArgumentException($"Class '{className}' not found in the current assembly.");
            }

            Speech = (ISpeech) Activator.CreateInstance(type);
        }
        catch (Exception e)
        {
            Logger.Critical($"Failed to instantiate speech implementation: {JsonSettings.speech_impl}");
            Logger.Critical(e.ToString());
            return false;
        }
        
        switch (Application.platform)
        {
            case RuntimePlatform.OSXPlayer:
                //Speech = new AppleSpeech();
                SpeechExtensions.AddUiElements<AppleVoiceUnity>(Constants.APPLE_VOICE_NAME);
                break;
            case RuntimePlatform.WindowsPlayer:
                //Speech = new WindowsSpeech();
                //Speech = new AuralisSpeech();
                //Speech = new KokoroSpeech();
                SpeechExtensions.AddUiElements<WindowsVoiceUnity>(Constants.WINDOWS_VOICE_NAME);
                break;
            default:
                // I'm not sure if this will ever run, as the Linux version does not exist.
                // Those running Linux use wine of some sort, which I believe would still show as Windows
                Logger.Critical($"SpeechMod is not supported on {Application.platform}!");
                return false;
        }

        return true;
    }

    private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
    {
        if (!value && Speech is IDisposable disposableSpeech)
        {
            disposableSpeech.Dispose();
            Speech = null;
        }
        Enabled = value;
        return true;
    }

    private static void OnGui(UnityModManager.ModEntry modEntry)
    {
        if (m_Loaded)
            MenuGUI.OnGui();
    }

    private static void OnSaveGui(UnityModManager.ModEntry modEntry)
    {
        MenuGUI.UpdateColors();
        Settings.Save(modEntry);
    }
}
