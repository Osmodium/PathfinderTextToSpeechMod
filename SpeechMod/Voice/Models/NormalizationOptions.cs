using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechMod.Voice.Models
{
    public class NormalizationOptions
    {
        public bool normalize {  get; set; }
        public bool unit_normalization { get; set; }
        public bool url_normalization { get; set; }
        public bool email_normalization { get; set; }
        public bool optional_pluralization_normalization { get; set; }
    }
}
