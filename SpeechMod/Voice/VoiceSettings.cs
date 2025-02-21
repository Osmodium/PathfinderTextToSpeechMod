using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kingmaker.Blueprints;
using Newtonsoft.Json;

namespace SpeechMod.voice;

public class VoiceSettings
{
    public string Narrator { get; set; }
    public int MinChars { get; set; }
    public bool Enabled { get; set; }
    public double Stability { get; set; }
    public double Style { get; set; }
    public bool UseSpeakerBoost { get; set; }
    public double SimilarityBoost { get; set; }
    public bool SaveAudio { get; set; }
    public string ApiKey { get; set; } = "";
    public List<string> GenericMaleVoices { get; set; } = [];
    public List<string> GenericFemaleVoices { get; set; } = [];
    public string Model { get; set; } = "";
    public Dictionary<string, string> NamedCharacters { get; set; } = new();
    public string AudioSavePath { get; set; }

    public static VoiceSettings Load(string path)
    {
        try
        {
            return JsonConvert.DeserializeObject<VoiceSettings>(File.ReadAllText(path));
        }
        catch (Exception e)
        {
            return new VoiceSettings();
        }
    }

    public string GetVoice(string name, Gender gender)
    {
        var lowerName = name.ToLower();
        
        if(name == Constants.Narrator)
            return Narrator;
        
        foreach (var key in NamedCharacters.Keys.Where(key => lowerName.Contains(key)))
        {
            if (!string.IsNullOrWhiteSpace(NamedCharacters[key])) 
                return NamedCharacters[key];
        }
        
        return GetConsistentGenericVoice(name, gender);
    }

    private string GetConsistentGenericVoice(string name, Gender gender)
    {
        var hash = name.GetHashCode();
        

        if (gender == Gender.Male)
        {
            if (GenericMaleVoices.Count == 0)
                return Narrator;
            
            var number = Math.Abs(hash % GenericMaleVoices.Count);
            return GenericMaleVoices.ElementAtOrDefault(number);
        }

        if (GenericFemaleVoices.Count == 0)
            return Narrator;
        
        var numberF = Math.Abs(hash % GenericFemaleVoices.Count);
        return GenericFemaleVoices.ElementAtOrDefault(numberF);
    }
}