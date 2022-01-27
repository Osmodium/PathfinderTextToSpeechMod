namespace SpeechMod.Voice;

public interface ISpeech
{
    string GetStatusMessage();
    string[] GetAvailableVoices();
    void Speak(string text, float delay = 0f);
    void Stop();
}