using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

public class WindowsVoice : MonoBehaviour {
  [DllImport("WindowsVoice")]
  public static extern void initSpeech();
  [DllImport("WindowsVoice")]
  public static extern void destroySpeech();
  [DllImport("WindowsVoice")]
  public static extern void addToSpeechQueue(string s);
  [DllImport("WindowsVoice")]
  public static extern void statusMessage(StringBuilder str, int length);

  public static WindowsVoice theVoice = null;
	// Use this for initialization
	void OnEnable () {
    if (theVoice == null)
    {
      theVoice = this;
      initSpeech();
    }
    //else
      //Destroy(gameObject);
	}
  public void test()
  {
    speak("Testing");
  }
  public void speak(string msg) {
    addToSpeechQueue(msg);
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
