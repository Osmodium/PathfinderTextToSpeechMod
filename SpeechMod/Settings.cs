using UnityModManagerNet;

namespace SpeechMod
{
    public class Settings : UnityModManager.ModSettings
    {
        public int Rate = 1;
        public int Volume = 100;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}
