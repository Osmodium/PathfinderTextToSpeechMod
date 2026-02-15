using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechMod.Voice.Models
{
    public class AuralisRequestItem
    {
        public string model { get; set; }
        public string[] voice { get; set; }
        public string input { get; set; }
        public string response_format { get; set; }
        public float speed { get; set; }

        public bool enhance_speech { get; set; }

        public bool sound_norm_refs { get; set; }

        public int max_ref_length { get; set; }
        public int gpt_cond_len { get; set; }
        public int gpt_cond_chunk_len { get; set; }
        public float temperature { get; set; }

        public float top_p { get; set; }
        public float top_k { get; set; }
        public float repetition_penalty { get; set; }
        public float length_penalty { get; set; }

        public bool do_sample { get; set; }

        public string language { get; set; }

    }
}
