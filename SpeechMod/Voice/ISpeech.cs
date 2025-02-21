namespace SpeechMod.Voice;

public interface ISpeech
{
    string GetStatusMessage();
    string[] GetAvailableVoices();
    void SpeakPreview(string text, VoiceType voiceType);
    void SpeakDialog(string text, float delay = 0f);
    void Speak(string text, float delay = 0f);
    void Stop();
}