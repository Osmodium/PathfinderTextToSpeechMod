
namespace SpeechMod.Voice;

public class WindowsSynthesis : ISpeech
{
    public string GetStatusMessage()
    {
        return "Test";
    }

    public string[] GetAvailableVoices()
    {
        return new []{""};
    }

    public void SpeakPreview(string text, VoiceType voiceType)
    {
        Speak(text);
    }

    public void SpeakDialog(string text, float delay = 0)
    {
        Speak(text, delay);
    }

    public void Speak(string text, float delay = 0)
    {

        //using SpeechSynthesizer synth = new SpeechSynthesizer();
        //synth.SetOutputToDefaultAudioDevice();
        //var color = new Prompt(text, SynthesisTextFormat.Ssml);
        //synth.Speak(color);
    }

    public void Stop()
    {
        //using SpeechSynthesizer synth = new SpeechSynthesizer();
        //synth.Pause();
    }
}