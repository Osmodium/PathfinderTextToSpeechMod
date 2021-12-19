using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SpeechMod.Unity;
using Debug = UnityEngine.Debug;

namespace SpeechMod.Voice
{
    public static class Speech
    {
        private static Dictionary<string, string> m_PhoneticDictionary;

        private static string Voice => $"<voice required=\"Name={ Main.ChosenVoice }\">";
        private static string Pitch => $"<pitch absmiddle=\"{ Main.Settings.Pitch }\"/>";
        private static string Rate => $"<rate absspeed=\"{ Main.Settings.Rate }\"/>";
        private static string Volume => $"<volume level=\"{ Main.Settings.Volume }\"/>";

        public static void LoadDictionary()
        {
            Main.Logger?.Log("Loading phonetic dictionary...");
            try
            {
                string file = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ?? throw new FileNotFoundException("Path to Pathfinder could not be found!"), @"Mods", @"SpeechMod", @"PhoneticDictionary.json");
                string json = File.ReadAllText(file, Encoding.UTF8);
                m_PhoneticDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            catch (Exception ex)
            {
                Main.Logger?.LogException(ex);
                Main.Logger?.Warning("Loading backup dictionary!");
                LoadBackupDictionary();
            }
#if DEBUG
            foreach (var entry in m_PhoneticDictionary)
            {
                Main.Logger.Log($"{entry.Key}={entry.Value}");
            }
#endif
        }

        public static bool IsSpeaking()
        {
            return WindowsVoiceUnity.IsSpeaking;
        }

        public static void Stop()
        {
            WindowsVoiceUnity.Stop();
        }

        public static void Speak(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Main.Logger?.Warning("No display text in the curren cue of the dialog controller!");
                return;
            }

#if DEBUG
            Debug.Log(text);
#endif
            text = text.Replace("\"", "");
            text = text.Replace("\n", ". ");
            
            text = m_PhoneticDictionary?.Aggregate(text, (current, pair) => current?.Replace(pair.Key, pair.Value));

            string textToSpeak = $"{ Voice }{ Pitch }{ Rate }{ Volume }{ text }</voice>";
#if DEBUG
            Debug.Log(textToSpeak);
#endif
            WindowsVoiceUnity.Speak(textToSpeak);
        }

        private static void LoadBackupDictionary()
        {
            m_PhoneticDictionary = new Dictionary<string, string>
            {
                { "—", "<silence msec=\"500\"/>" },
                { "Kenabres", "Ken-aabres" },
                { "Iomedae", "I-o-mædæ" },
                { "Golarion", "Goolaarion" },
                { "Sovyrian", "Sovyyrian" },
                { "Rovagug", "Rovaagug" },
                { "Irabeth", "Iira-beth" },
                { "Terendelev", "Ter-end-elev" },
                { "Arendae", "Aren-dæ" },
                { "tieflings", "teeflings" },
                { "Deskari", "Dess-kaari "}
            };
        }
    }
}
