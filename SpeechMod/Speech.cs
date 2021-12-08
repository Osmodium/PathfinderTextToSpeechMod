using System.Collections.Generic;
using UnityEngine;

namespace SpeechMod
{
    public static class Speech
    {
        private static readonly Dictionary<string, string> _phoneticalDictionary = new Dictionary<string, string>()
        {
            { "—", "," },
            { "Kenabres", "Keenaaabres" },
            { "Iomedae", "I,omedaee" },
            { "Golarion", "Goolaarion" },
            { "Sovyrian", "Sovyyrian" },
            { "Rovagug", "Rovaagug" },
            { "Irabeth", "Iira,beth" }
        };

        public static void Speak(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogWarning("No display text in the curren cue of the dialog controller!");
                return;
            }

            // TODO: Load replaces into a dictionary from a json file so they can be added and altered more easily.
            foreach (var pair in _phoneticalDictionary)
            {
                text = text.Replace(pair.Key, pair.Value);
            }

            WindowsVoice.Speak($"<voice required=\"Name = { Main.ChosenVoice }\"><pitch absmiddle = \"{ Main.Settings.Pitch }\" >{ text }");
        }
    }
}
