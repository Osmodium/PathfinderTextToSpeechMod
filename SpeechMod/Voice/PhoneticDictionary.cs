using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpeechMod.Voice;

public static class PhoneticDictionary
{
    private static Dictionary<string, string> s_PhoneticDictionary = new();

    private static string SpaceOutDate(string text)
    {
        var pattern = @"([0-9]{2})\/([0-9]{2})\/([0-9]{4})";
        return Regex.Replace(text, pattern, "$1 / $2 / $3");
    }

    public static string PrepareText(this string text)
    {
        if (s_PhoneticDictionary == null || !s_PhoneticDictionary.Any())
            LoadDictionary();

        text = text.ToLower();
        text = text.Replace("\"", "");
        text = text.Replace("\r\n", ". ");
        text = text.Replace("\n", ". ");
        text = text.Replace("\r", ". ");
        text = text.Trim();
        text = SpaceOutDate(text);

        // Regex enabled dictionary
        return s_PhoneticDictionary?.Aggregate(text, (current, entry) => Regex.Replace(current, entry.Key, entry.Value));
    }

    public static void LoadDictionary()
    {
        Main.Logger?.Log("Loading phonetic dictionary...");
        try
        {
            var file = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName) ?? throw new FileNotFoundException("Path to Pathfinder could not be found!"), @"Mods", @"SpeechMod", @"PhoneticDictionary.json");
            var json = File.ReadAllText(file, Encoding.UTF8);
            s_PhoneticDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }
        catch (Exception ex)
        {
            Main.Logger?.LogException(ex);
        }

#if DEBUG
        foreach (var entry in s_PhoneticDictionary)
        {
            Main.Logger?.Log($"{entry.Key}={entry.Value}");
        }
#endif
    }
}