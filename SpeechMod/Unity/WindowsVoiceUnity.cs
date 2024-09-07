using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SpeechMod.Unity;

public class WindowsVoiceUnity : MonoBehaviour
{
    public enum WindowsVoiceStatus { Uninitialized, Ready, Speaking, Terminated, Error }

    [DllImport(Constants.WINDOWS_VOICE_DLL)]
    private static extern void initSpeech(int rate, int volume);
    [DllImport(Constants.WINDOWS_VOICE_DLL)]
    private static extern void destroySpeech();
    [DllImport(Constants.WINDOWS_VOICE_DLL)]
    private static extern void addToSpeechQueue(string s);
    [DllImport(Constants.WINDOWS_VOICE_DLL)]
    private static extern void clearSpeechQueue();
    [DllImport(Constants.WINDOWS_VOICE_DLL)]
    private static extern string getStatusMessage();
    [DllImport(Constants.WINDOWS_VOICE_DLL)]
    private static extern string getVoicesAvailable();
    [DllImport(Constants.WINDOWS_VOICE_DLL)]
    private static extern int getWordLength();
    [DllImport(Constants.WINDOWS_VOICE_DLL)]
    private static extern int getWordPosition();
    [DllImport(Constants.WINDOWS_VOICE_DLL)]
    private static extern WindowsVoiceStatus getSpeechState();

    private static WindowsVoiceUnity m_TheVoice;
    private static int m_CurrentWordCount;

    public static bool IsSpeaking => getSpeechState() == WindowsVoiceStatus.Speaking;
    public static WindowsVoiceStatus VoiceStatus => getSpeechState();

    private static void Init()
    {
        Main.Logger.Log("Init");
        initSpeech(1, 100);
    }
    private static bool IsVoiceInitialized()
    {
        if (m_TheVoice != null)
            return true;

        Main.Logger.Critical("No voice initialized!");
        return false;
    }

    void Start()
    {
        m_CurrentWordCount = 0;
        if (m_TheVoice != null)
        {
            Destroy(gameObject);
        }
        else
        {
            m_TheVoice = this;
            Init();
        }
    }

    public static string[] GetAvailableVoices()
    {
        string voicesDelim = getVoicesAvailable();
        if (string.IsNullOrWhiteSpace(voicesDelim))
            return Array.Empty<string>();
        string[] voices = voicesDelim.Split(['\n'], StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < voices.Length; ++i)
        {
            if (!voices[i].Contains('-'))
                voices[i] = $"{voices[i]}#Unknown";
            else
                voices[i] = voices[i].Replace(" - ", "#");

            if (!voices[i].Contains("(Natural)"))
                continue;

            voices[i] = voices[i].Replace("(Natural)", "");
            voices[i] = voices[i].Replace("(", "Natural (");
        }
        return voices;
    }

    public static void Speak(string text, int length, float delay = 0f)
    {
	    Main.Logger.Log("Speak");
		try
	    {
		    if (!IsVoiceInitialized())
			    return;

		    if (Main.Settings.InterruptPlaybackOnPlay && IsSpeaking)
			    Stop();

		    m_CurrentWordCount = length;
		    if (delay <= 0f)
			    addToSpeechQueue(text);
		    else
			    m_TheVoice.ExecuteLater(delay, () => Speak(text, length));
	    }
	    catch (Exception e)
	    {
            Main.Logger.Error(e.Message + e.StackTrace);
	    }
    }

    public static string GetStatusMessage()
    {
        return getStatusMessage();
    }

    public static int WordPosition => getWordPosition();

    public static int WordCount => m_CurrentWordCount;

    public static int WordLength => getWordLength();

    public static float GetNormalizedProgress()
    {
        return 1-(float)(m_CurrentWordCount - getWordPosition()) / m_CurrentWordCount;
    }

    public static void Stop()
    {
        Main.Logger.Log("Stop");

        try
        {
	        if (!IsVoiceInitialized())
		        return;

	        if (!IsSpeaking)
		        return;

	        destroySpeech();
	        Init();
        }
        catch (Exception ex)
        {
	        Main.Logger.Error(ex.Message + ex.StackTrace);
        }
    }

    public static void ClearQueue()
    {
        clearSpeechQueue();
    }

    void OnDestroy()
    {
        if (m_TheVoice != this)
            return;

        Debug.Log("Destroying speech");
        destroySpeech();
        Debug.Log("Speech destroyed");
        m_TheVoice = null;
    }
}