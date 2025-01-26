namespace SpeechMod.Voice;

public interface ISpeech
{
    string GetStatusMessage();
    string[] GetAvailableVoices();
    bool IsSpeaking();
    void SpeakPreview(string text, VoiceType voiceType);
    void SpeakDialog(string text, float delay = 0f);
    void SpeakAs(string text, VoiceType voiceType, float delay = 0f);
    void Speak(string text, float delay = 0f);
    void Stop();
}