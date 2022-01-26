namespace SpeechMod.Voice;

public interface ISpeech
{
    void Speak(string text);
    string[] GetAvailableVoices();
    string GetStatusMessage();
}