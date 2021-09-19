using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace SpeechMod
{
#if (DEBUG)
    [EnableReloading]
#endif
    internal static class Main
    {
        public static UnityModManager.ModEntry.ModLogger Logger;
        public static Settings Settings;
        public static bool Enabled;

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            Logger = modEntry.Logger;

            Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGui;
            modEntry.OnSaveGUI = OnSaveGui;

            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            
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
            GUILayout.Label("Speech Volume", GUILayout.ExpandWidth(false));
            GUILayout.Space(10);
            Settings.Volume = (int)GUILayout.HorizontalSlider(Settings.Volume, 0, 100, GUILayout.Width(300f));
            GUILayout.Label($" {Settings.Volume}", GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
        }

        private static void OnSaveGui(UnityModManager.ModEntry modEntry)
        {
            Settings.Save(modEntry);
        }
    }
}
