using SpeechMod;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// Credit to Chad Weisshaar for the base from https://chadweisshaar.com/blog/2015/07/02/microsoft-speech-for-unity/
/// </summary>

#if FAKE_WINDOWS_VOICE
public class WindowsVoice : MonoBehaviour
{
  public TextMeshProUGUI DebugOutput = null;

  public static WindowsVoice theVoice = null;
  void OnEnable () 
  {
    if (theVoice == null)
      theVoice = this;
  }
  public static void speak(string msg, float delay = 0f) {
    if (Timeline.theTimeline.QReprocessingEvents)
      return;

    if (delay == 0f)
    {
      if (theVoice.DebugOutput != null)
        theVoice.DebugOutput.text = msg;
      else
        Debug.Log("SPEAK: " + msg);
    }
    else
      theVoice.ExecuteLater(delay, () => speak(msg));
  }
}
#else
public static class Utility
{
    public static Coroutine ExecuteLater(this MonoBehaviour behaviour, float delay, Action fn)
    {
        return behaviour.StartCoroutine(_realExecute(delay, fn));
    }
    static IEnumerator _realExecute(float delay, Action fn)
    {
        yield return new WaitForSeconds(delay);
        fn();
    }
}

public class WindowsVoiceUnity : MonoBehaviour
{
    [DllImport("WindowsVoice")]
    private static extern void initSpeech(int rate, int volume);
    [DllImport("WindowsVoice")]
    private static extern void destroySpeech();
    [DllImport("WindowsVoice")]
    private static extern void addToSpeechQueue(string s);
    [DllImport("WindowsVoice")]
    private static extern void clearSpeechQueue();
    [DllImport("WindowsVoice")]
    private static extern string getStatusMessage();
    [DllImport("WindowsVoice")]
    private static extern string getVoicesAvailable();

    public static WindowsVoiceUnity theVoice = null;

    void Start()
    {
        if (theVoice == null)
        {
            theVoice = this;
            initSpeech(1, 100);
        }
        //else
        //Destroy(gameObject);
    }

    public void Test()
    {
        Speak("Testing");
    }

    public static string[] GetAvailableVoices()
    {
        Main.Logger.Log("GetAvailableVoices");
        string voicesDelim = getVoicesAvailable();
        if (string.IsNullOrWhiteSpace(voicesDelim))
            return Array.Empty<string>();
        string[] voices = voicesDelim.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for(int i = 0; i < voices.Length; ++i)
        {
            if (!voices[i].Contains('-'))
                continue;
            voices[i] = voices[i].Substring(0, voices[i].IndexOf('-')).Trim();
        }
        return voices;
    }

    public static void Speak(string msg, float delay = 0f)
    {
        if (delay == 0f)
            addToSpeechQueue(msg);
        else
            theVoice.ExecuteLater(delay, () => Speak(msg));
    }

    public static string GetStatusMessage()
    {
        return getStatusMessage();
    }

    void OnDestroy()
    {
        if (theVoice == this)
        {
            Debug.Log("Destroying speech");
            destroySpeech();
            Debug.Log("Speech destroyed");
            theVoice = null;
        }
    }
}
#endif