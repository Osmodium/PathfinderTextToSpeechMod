using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SpeechMod.Voice;

public static class SpeechExtensions
{
    public static void AddUiElements<T>(string name) where T : MonoBehaviour
    {
        GameObject voiceGameObject = null;
        try
        {
            voiceGameObject = Object.FindObjectOfType<T>()?.gameObject;
        }
        catch
        {
            // Ignored
        }

        if (voiceGameObject != null)
        {
            Debug.Log($"{typeof(T).Name} found, returning!");
            return;
        }

        Debug.Log($"Adding {typeof(T).Name} SpeechMod UI elements.");

        var windowsVoiceGameObject = new GameObject(name);
        windowsVoiceGameObject.AddComponent<T>();
        Object.DontDestroyOnLoad(windowsVoiceGameObject);
    }
}