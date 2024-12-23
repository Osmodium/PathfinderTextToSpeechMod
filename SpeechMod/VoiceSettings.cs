using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kingmaker.Blueprints;
using Newtonsoft.Json;

namespace SpeechMod;

public class VoiceSettings
{
  public double Stability { get; set; }
  public double Style { get; set; }
  public bool UseSpeakerBoost { get; set; }
  public double SimilarityBoost { get; set; }
  public bool SaveAudio { get; set; }
  public string ApiKey { get; set; } = "";
  public List<string> GenericMaleVoices { get; set; } = new();
  public List<string> GenericFemaleVoices { get; set; } = new();
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
    foreach (var key in NamedCharacters.Keys.Where(key => lowerName.Contains(key)))
    {
      return NamedCharacters[key];
    }

    return GetConsistentGenericVoice(name, gender);
  }

  private string GetConsistentGenericVoice(string name, Gender gender)
  {
    var hash = name.GetHashCode();

    if (gender == Gender.Male)
    {
      var number = Math.Abs(hash % GenericMaleVoices.Count);
      return GenericMaleVoices.ElementAtOrDefault(number);
    }

    var numberF = Math.Abs(hash % GenericFemaleVoices.Count);
    return GenericFemaleVoices.ElementAtOrDefault(numberF);
  }
}