using UnityModManagerNet;

namespace SpeechMod;

public class Settings : UnityModManager.ModSettings
{
    public int Rate = 0;
    public int Volume = 100;
    public int Pitch = 0;

    public string[] AvailableVoices;
    public int ChosenVoice = 1;

    public bool ColorOnHover = false;
    public float HoverColorR = 0f;
    public float HoverColorG = 0f;
    public float HoverColorB = 0f;
    public float HoverColorA = 1;

    public bool FontStyleOnHover = true;
    public bool[] FontStyles = { false, false, false, true, false, false, false, false, false, false, false };

    public bool ShowPlaybackProgress = true;
    public float PlaybackColorR = 0.7f;
    public float PlaybackColorG = 0.9f;
    public float PlaybackColorB = 0.7f;
    public float PlaybackColorA = 0.5f;

    public bool InterruptPlaybackOnPlay = true;

    public override void Save(UnityModManager.ModEntry modEntry)
    {
        Save(this, modEntry);
    }
}