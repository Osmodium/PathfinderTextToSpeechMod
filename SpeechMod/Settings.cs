using UnityModManagerNet;

namespace SpeechMod
{
    public class Settings : UnityModManager.ModSettings
    {
        public int Rate = 1;
        public int Volume = 100;

        public string[] AvailableVoices = { "Microsoft Hazel Desktop", "Microsoft David Desktop", "Microsoft Zira Desktop" };
        public int ChosenVoice = 1;

        public bool ColorOnHover = false;
        public float ChosenColorR = 0f;
        public float ChosenColorG = 0f;
        public float ChosenColorB = 0f;
        public float ChosenColorA = 1;

        public bool FontStyleOnHover = true;
        public bool[] FontStyles = { false, false, false, true, false, false, false, false, false, false, false };

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}
