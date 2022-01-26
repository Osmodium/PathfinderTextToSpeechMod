namespace SpeechMod.Voice;

public interface ISpeech
{
    void Speak(string text, float delay = 0f);
    string[] GetAvailableVoices();
    string GetStatusMessage();
}