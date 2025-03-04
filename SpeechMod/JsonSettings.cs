namespace SpeechMod;

// These default settings will normally be overwritten upon deserialization from settings.json
public class JsonSettings
{
    // AuralisSpeech, KokoroSpeech, AppleSpeech, WindowsSpeech
    public string speech_impl = "AuralisSpeech";

    public string endpoint = "http://127.0.0.1:8000/v1/audio/speech";

    // possibly create setting for audio file download location
    //public string audio_file_download_location = Path.GetTempPath();

    // Auralis-specific settings
    public AuralisJsonSettings auralis_settings = new AuralisJsonSettings();

    // Kokoro-specific settings
    public KokoroJsonSettings kokoro_settings = new KokoroJsonSettings();

}

public class AuralisJsonSettings
{
    public string path_to_voice_one_shot = "female_01.wav";
    public string response_format = "wav";
    public float speed = 1.0f;
    public string model = "xttsv2";
    public bool enhance_speech = true;
    public bool sound_norm_refs = false;
    public int max_ref_length = 60;
    public int gpt_cond_len = 30;
    public int gpt_cond_chunk_len = 4;
    public float temperature = 0.75f;
    public float top_p = 0.85f;
    public int top_k = 50;
    public float repetition_penalty = 5.0f;
    public float length_penalty = 1.0f;
    public bool do_sample = true;
    public string language = "auto";
    
}

public class KokoroJsonSettings
{
    public string voice = "af_heart";
    public string model = "kokoro";
    public float speed = 1.0f;
    public string lang_code = "a";
    public string response_format = "wav";

    // Normalization options
    public bool normalize = true;
    public bool unit_normalization = false;
    public bool url_normalization = true;
    public bool email_normalization = true;
    public bool optional_pluralization_normalization = true;
}
