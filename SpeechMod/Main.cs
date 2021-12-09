using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using TMPro;
using System;
using System.Linq;

namespace SpeechMod
{
#if DEBUG
    [EnableReloading]
#endif
    internal static class Main
    {
        public static UnityModManager.ModEntry.ModLogger Logger;
        public static Settings Settings;
        public static bool Enabled;

        public static string[] FontStyleNames = Enum.GetNames(typeof(FontStyles));
        
        public static string ChosenVoice
        {
            get { return Settings.AvailableVoices[Settings.ChosenVoice]; }
        }

        public static Color ChosenColor
        {
            get { return new Color(Settings.ChosenColorR, Settings.ChosenColorG, Settings.ChosenColorB, Settings.ChosenColorA); }
        }

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            Debug.Log("Speech Mod Initializing...");

            Logger = modEntry.Logger;

            Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);

            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGui;
            modEntry.OnSaveGUI = OnSaveGui;

            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Logger.Log(WindowsVoiceUnity.GetStatusMessage());

            Logger.Log("Available voices:");
            string[] availableVoices = WindowsVoiceUnity.GetAvailableVoices();
            foreach (var s in availableVoices)
            {
                Logger.Log(s);
            }
            if (availableVoices == null || availableVoices.Length == 0)
            { 
                Logger.Warning("No voices available found! Diabling mod!");
                return false;
            }
            Logger.Log("Setting available voices list...");
            Settings.AvailableVoices = availableVoices;

            Debug.Log("Speech Mod Initialized!");

            return true;
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Enabled = value;
            return true;
        }

        private static void OnGui(UnityModManager.ModEntry modEntry)
        {
            // TODO list of voices to choose from.

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
            //GUILayout.Label($"{ChosenVoice}", GUILayout.ExpandWidth(false));
            Settings.ChosenVoice = GUILayout.SelectionGrid(Settings.ChosenVoice, Settings.AvailableVoices, 3);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Settings.ColorOnHover = GUILayout.Toggle(Settings.ColorOnHover, "Enable color on hover");
            GUILayout.EndHorizontal();

            if (Settings.ColorOnHover)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Hover color", GUILayout.ExpandWidth(false));
                GUILayout.Space(10);
                GUILayout.Label("R ", GUILayout.ExpandWidth(false));
                Settings.ChosenColorR = GUILayout.HorizontalSlider(Settings.ChosenColorR, 0, 1);
                GUILayout.Space(10);
                GUILayout.Label("G", GUILayout.ExpandWidth(false));
                Settings.ChosenColorG = GUILayout.HorizontalSlider(Settings.ChosenColorG, 0, 1);
                GUILayout.Space(10);
                GUILayout.Label("B", GUILayout.ExpandWidth(false));
                Settings.ChosenColorB = GUILayout.HorizontalSlider(Settings.ChosenColorB, 0, 1);
                GUILayout.Space(10);
                GUILayout.Label("A", GUILayout.ExpandWidth(false));
                Settings.ChosenColorA = GUILayout.HorizontalSlider(Settings.ChosenColorA, 0, 1);
                GUILayout.Space(10);
                GUILayout.Box(ColorPreview, GUILayout.Width(20));
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            Settings.FontStyleOnHover = GUILayout.Toggle(Settings.FontStyleOnHover, "Enable font style on hover");
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
        }

        private static Texture2D ColorPreview
        {
            get
            {
                var texture = new Texture2D(20, 20);
                for (int y = 0; y < texture.height; y++)
                {
                    for (int x = 0; x < texture.width; x++)
                    {
                        texture.SetPixel(x, y, ChosenColor);
                    }
                }
                texture.Apply();
                return texture;
            }
        }

        private static void OnSaveGui(UnityModManager.ModEntry modEntry)
        {
            Settings.Save(modEntry);
        }
    }
}
