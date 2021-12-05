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
            { "Sovyrian", "Sovyyrian" }
        };

        public static void Speak(string text)
        {
            //var text = Game.Instance?.DialogController?.CurrentCue?.DisplayText;
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

            WindowsVoice.Speak(text);
        }
    }
}
