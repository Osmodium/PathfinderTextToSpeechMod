using HarmonyLib;
using SpeechMod.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NAudio.Wave;
using SpeechMod.voice;
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
    public static WaveOutEvent WaveOutEvent = new();
    public static VoiceSettings VoiceSettings;

    public static string[] FontStyleNames = Enum.GetNames(typeof(FontStyles));

    private static bool m_Loaded = false;

    private static bool Load(UnityModManager.ModEntry modEntry)
    {
        VoiceSettings = VoiceSettings.Load($"{modEntry.Path}/settings.json");
        Debug.Log("Pathfinder: Wrath of the Righteous Speech Mod Initializing...");

        Logger = modEntry.Logger;

        //Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);

        var harmony = new Harmony(modEntry.Info.Id);
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        Debug.Log("Speech Mod Initialized!");
        m_Loaded = true;
        return true;
    }

}