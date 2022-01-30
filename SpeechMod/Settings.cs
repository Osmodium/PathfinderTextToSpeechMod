using UnityModManagerNet;

namespace SpeechMod;

public class Settings : UnityModManager.ModSettings
{
    public int NarratorRate = 0;
    public int NarratorVolume = 100;
    public int NarratorPitch = 0;

    public int FemaleRate = 0;
    public int FemaleVolume = 100;
    public int FemalePitch = 0;

    public int MaleRate = 0;
    public int MaleVolume = 100;
    public int MalePitch = 0;
        
    public bool AutoPlay = false;

    public string[] AvailableVoices;
    public int NarratorVoice = 1;

    public bool UseGenderSpecificVoices = false;
    public int FemaleVoice = 0;
    public int MaleVoice = 2;

    public bool ColorOnHover = false;
    public float HoverColorR = 0f;
    public float HoverColorG = 0f;
    public float HoverColorB = 0f;
    public float HoverColorA = 1;

    public bool FontStyleOnHover = true;
    public bool[] FontStyles = { false, false, false, true, false, false, false, false, false, false, false };

    //public bool ShowPlaybackProgress = true;
    //public float PlaybackColorR = 0.7f;
    //public float PlaybackColorG = 0.9f;
    //public float PlaybackColorB = 0.7f;
    //public float PlaybackColorA = 0.5f;

    public bool InterruptPlaybackOnPlay = true;

    public override void Save(UnityModManager.ModEntry modEntry)
    {
        Save(this, modEntry);
    }
}
