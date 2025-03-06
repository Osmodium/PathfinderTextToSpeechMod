using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechMod.Voice.Models
{
    public class KokoroRequestItem
    {
        public string model { get; set; }
        public string voice { get; set; }
        public string input { get; set; }
        public string response_format { get; set; }
        public float speed { get; set; }
        public bool stream { get; set; }
        public bool return_download_link { get; set; }
        public string lang_code { get; set; }
        public NormalizationOptions normalization_options { get; set; }

    }
}
