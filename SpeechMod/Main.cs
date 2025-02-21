using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NAudio.Wave;
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
    public static bool Enabled;
    public static WaveOutEvent WaveOutEvent = new ();
    public static VoiceSettings VoiceSettings;   

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
    
    private static bool m_Loaded = false;

    private static bool Load(UnityModManager.ModEntry modEntry)
    {
        VoiceSettings = VoiceSettings.Load($"{modEntry.Path}/settings.json");
        Debug.Log("Pathfinder: Wrath of the Righteous Speech Mod Initializing...");

        Logger = modEntry.Logger;

        var harmony = new Harmony(modEntry.Info.Id);
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        Debug.Log("Speech Mod Initialized!");
        m_Loaded = true;
        return true;
    }
}
