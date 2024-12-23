using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpeechMod;

public static class TextParser
{
  public static (string finalName, string text) MakeTextForVoice(string text, string name)
  {
    if (name == "narrator")
      return (name, text);

    var (narratorValues, speakerValues) = SeparateNarratorAndSpeaker(text);


    var joinedNarratorValues = string.Join(", ", narratorValues);
    var joinedSpeakerValues = string.Join("... ...", speakerValues); // ellipsis is for a pause when sent to the TTS

    return speakerValues.Count == 0
      ? ("narrator", joinedNarratorValues) 
      : (name, joinedSpeakerValues);
  }

  private static (List<string> NarratorValues, List<string> SpeakerValues) SeparateNarratorAndSpeaker(string input)
  {
    List<string> Extract(MatchCollection collection) =>
      collection
        .Cast<Match>()
        .Select(match => match.Groups[1].Value)
        .ToList();

    // Regex to match NarratorValues
    var narratorRegex = new Regex(@"<color=#616060>(.*?)</color>");
    var narratorMatches = narratorRegex.Matches(input);

    var narratorValues = Extract(narratorMatches);

    // Regex to match SpeakerValues
    var speakerRegex = new Regex("\"(.*?)\"");
    var speakerMatches = speakerRegex.Matches(input);

    var speakerValues = Extract(speakerMatches);

    return (narratorValues, speakerValues);
  }
}