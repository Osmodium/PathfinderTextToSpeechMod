using SpeechMod;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

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
    public static Coroutine ExecuteLater(this MonoBehaviour behaviour, float delay, System.Action fn)
    {
        return behaviour.StartCoroutine(_realExecute(delay, fn));
    }
    static IEnumerator _realExecute(float delay, System.Action fn)
    {
        yield return new WaitForSeconds(delay);
        fn();
    }
}

public class WindowsVoice : MonoBehaviour
{
    [DllImport("WindowsVoice")]
    public static extern void initSpeech(int rate, int volume);
    [DllImport("WindowsVoice")]
    public static extern void destroySpeech();
    [DllImport("WindowsVoice")]
    public static extern void addToSpeechQueue(string s);
    [DllImport("WindowsVoice")]
    public static extern void clearSpeechQueue();
    [DllImport("WindowsVoice")]
    public static extern void statusMessage(StringBuilder str, int length);

    public static WindowsVoice theVoice = null;

    void Start()
    {
        if (theVoice == null)
        {
            theVoice = this;
            initSpeech(Main.Settings?.Rate ?? -1, Main.Settings?.Volume ?? 100);
        }
        //else
        //Destroy(gameObject);
    }

    public void Test()
    {
        Speak("Testing");
    }

    public static void Speak(string msg, float delay = 0f)
    {
        if (delay == 0f)
            addToSpeechQueue(msg);
        else
            theVoice.ExecuteLater(delay, () => Speak(msg));
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

    public static string GetStatusMessage()
    {
        StringBuilder sb = new StringBuilder(40);
        statusMessage(sb, 40);
        return sb.ToString();
    }
}
#endif