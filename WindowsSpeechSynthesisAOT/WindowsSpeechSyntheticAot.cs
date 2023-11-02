using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;

namespace WindowsSpeechSynthesisAOT;

public static class WindowsSpeechSyntheticAot
{

#if WINDOWS
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) }, EntryPoint = "Speak")]
    public static void Speak(IntPtr textPtr)
    {
        var text = Marshal.PtrToStringUTF8(textPtr);

        if (string.IsNullOrEmpty(text))
            return;

        var t = text.Replace(" ", "");

        if (string.IsNullOrEmpty(t))
            return;

        using SpeechSynthesizer synth = new SpeechSynthesizer();
        synth.SetOutputToDefaultAudioDevice();
        var color = new Prompt(text, SynthesisTextFormat.Ssml);
        synth.Speak(color);
    }
#endif

}