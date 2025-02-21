using System;
using System.IO;
using System.Threading.Tasks;
using Kingmaker.Blueprints;
using NAudio.Wave;
using SpeechMod.ElevenLabs;
using UnityEngine;

namespace SpeechMod.voice;

public class VoicePlayer
{
  private static async Task PlayStream(Stream stream, string key)
  {
    using var waveOut = Main.WaveOutEvent;

    if (waveOut.PlaybackState == PlaybackState.Playing)
      waveOut.Stop();

    try
    {
      using var mp3Reader = new Mp3FileReader(stream);

      waveOut.Init(mp3Reader);
      waveOut.Play();

      while (waveOut.PlaybackState == PlaybackState.Playing)
      {
        await Task.Delay(1000);
      }

      if (Main.VoiceSettings.SaveAudio)
        SaveToFile(stream, key);
    }
    catch (Exception e)
    {
      Debug.Log("error playing stream");
      Debug.Log(e.Message);
    }
  }

  private static async Task PlayExistingMp3(string filePath)
  {
    try
    {
      using var waveOut = Main.WaveOutEvent;

      if (waveOut.PlaybackState == PlaybackState.Playing)
        waveOut.Stop();

      using var audioFile = new Mp3FileReader(filePath);

      waveOut.Init(audioFile);
      waveOut.Play();

      while (waveOut.PlaybackState == PlaybackState.Playing)
      {
        await Task.Delay(1000);
      }
    }
    catch (Exception e)
    {
      Debug.Log("error playing audio");
      Debug.Log(e.Message);
    }
  }

  private static void SaveToFile(Stream stream, string key)
  {
    using var fileStream = new FileStream($@"{Main.VoiceSettings.AudioSavePath}\{key}.mp3", FileMode.Create);
    stream.Seek(0, SeekOrigin.Begin);
    stream.CopyTo(fileStream);
  }

  public static async Task PlayText(string text, string key, Gender gender, string speaker)
  {
    var fullPath = "";
    var fileExists = false;
    
    try
    {
      fullPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), $@"{Main.VoiceSettings.AudioSavePath}\{key}.mp3");
      fileExists = System.IO.File.Exists(fullPath);
    }
    catch
    {
      // ignored
    }

    if (fileExists)
    {
      await PlayExistingMp3(fullPath);
    }
    else
    {
      var (finalName, textForVoice) = TextParser.MakeTextForVoice(text, speaker);
      var voice = Main.VoiceSettings.GetVoice(finalName, gender);
      
      if(textForVoice.Length < Main.VoiceSettings.MinChars)
        return;
      
      ElevenReq req = new()
      {
        ModelID = Main.VoiceSettings.Model,
        Text = textForVoice,
        Voice = voice
      };

      var stream = await ElevenLabsGateway.CreateStream(req);

      if (stream != null)
        await PlayStream(stream, key);
    }
  }
}