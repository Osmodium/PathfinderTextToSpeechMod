using System;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;
using UnityEngine;

namespace SpeechMod;

public class VoicePlayer
{
  public static async Task PlayStream(Stream stream, string key)
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

  public static async Task PlayExistingMp3(string filePath)
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
}