using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using SpeechMod.Unity;
using Debug = UnityEngine.Debug;

namespace SpeechMod.Speech
{
    public static class Speech
    {
        private static Dictionary<string, string> _phoneticDictionary;

        private static string _voice => $"<voice required=\"Name={ Main.ChosenVoice }\">";
        private static string _pitch => $"<pitch absmiddle=\"{ Main.Settings.Pitch }\"/>";
        private static string _rate => $"<rate absspeed=\"{ Main.Settings.Rate }\"/>";
        private static string _volume => $"<volume level=\"{ Main.Settings.Volume }\"/>";

        public static void LoadDictionary()
        {
            try
            {
                string file = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName), @"Mods", @"SpeechMod", @"PhoneticDictionary.json");
                string json = File.ReadAllText(file, Encoding.UTF8);
                _phoneticDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            catch (Exception ex)
            {
                Main.Logger.LogException(ex);
                LoadBackupDictionary();
            }
        }

        public static void Speak(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogWarning("No display text in the curren cue of the dialog controller!");
                return;
            }

            // TODO: Load replaces into a dictionary from a json file so they can be added and altered more easily.
            foreach (var pair in _phoneticDictionary)
            {
                text = text.Replace(pair.Key, pair.Value);
            }

            string textToSpeak = $"{ _voice }{ _pitch }{ _rate }{ _volume }{ text }</voice>";
#if DEBUG
            Main.Logger.Log(textToSpeak);
#endif
            WindowsVoiceUnity.Speak(textToSpeak);
        }

        private static void LoadBackupDictionary()
        {
            _phoneticDictionary = new Dictionary<string, string>
            {
                { "—", "," },
                { "Kenabres", "Keenaaabres" },
                { "Iomedae", "I,omedaee" },
                { "Golarion", "Goolaarion" },
                { "Sovyrian", "Sovyyrian" },
                { "Rovagug", "Rovaagug" },
                { "Irabeth", "Iira,beth" }
            };
        }
    }
}
