using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SpeechMod.Voice;

public static class SpeechExtensions
{
    private static Dictionary<string, string> m_PhoneticDictionary;

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

    public static string PrepareText(this string text)
    {
        text = text.Replace("\"", "");
        text = text.Replace("\n", ". ");
        text = text.Trim().Trim('.');

        if (m_PhoneticDictionary == null)
            LoadBackupDictionary();

        return m_PhoneticDictionary.Aggregate(text, (current, pair) => current?.Replace(pair.Key, pair.Value));
    }

    public static void AddUiElements<T>(string name) where T : MonoBehaviour
    {
        Debug.Log($"Adding {name} SpeechMod UI elements.");

        GameObject voice = null;
        try
        {
            voice = UnityEngine.Object.FindObjectOfType<T>()?.gameObject;
        }
        catch{} // Sigh

        if (voice != null)
        {
            Debug.Log($"{typeof(T).Name} found!");
            return;
        }

        Debug.Log($"Adding {typeof(T).Name}...");

        var windowsVoiceGameObject = new GameObject(name);
        windowsVoiceGameObject.AddComponent<T>();
        UnityEngine.Object.DontDestroyOnLoad(windowsVoiceGameObject);
    }
}