using Newtonsoft.Json;
using SpeechMod.Unity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NAudio.Wave;
using System.Threading;
using SpeechMod.Voice.Models;

namespace SpeechMod.Voice
{
    public class AuralisSpeech : APISpeech
    {
        public AuralisSpeech() : base(){}
       
        protected override void ProcessAndQueueFile(string item, int count)
        {
            var jsonSettings = Main.JsonSettings;

            byte[] fileBytes = File.ReadAllBytes(jsonSettings.auralis_settings.path_to_voice_one_shot);
            string base64String = Convert.ToBase64String(fileBytes);

            var reqItem = new AuralisRequestItem();
            reqItem.voice = new string[] { base64String };
            reqItem.input = item;
            
            reqItem.response_format = jsonSettings.auralis_settings.response_format;
            reqItem.speed = jsonSettings.auralis_settings.speed;
            reqItem.model = jsonSettings.auralis_settings.model;

            reqItem.enhance_speech = jsonSettings.auralis_settings.enhance_speech;
            reqItem.sound_norm_refs = jsonSettings.auralis_settings.sound_norm_refs;
            reqItem.max_ref_length = jsonSettings.auralis_settings.max_ref_length;
            reqItem.gpt_cond_len = jsonSettings.auralis_settings.gpt_cond_len;
            reqItem.gpt_cond_chunk_len = jsonSettings.auralis_settings.gpt_cond_chunk_len;
            reqItem.temperature = jsonSettings.auralis_settings.temperature;
            reqItem.top_p = jsonSettings.auralis_settings.top_p;
            reqItem.top_k = jsonSettings.auralis_settings.top_k;
            reqItem.repetition_penalty = jsonSettings.auralis_settings.repetition_penalty;
            reqItem.length_penalty = jsonSettings.auralis_settings.length_penalty;
            reqItem.do_sample = jsonSettings.auralis_settings.do_sample;
            reqItem.language = jsonSettings.auralis_settings.language;

            var content = JsonContent.Create(reqItem);

            // since this already runs in a seperate thread, these calls to the TTS service will run synchronously
            var task = Task.Run(() => sharedHttpClient.PostAsync(jsonSettings.endpoint, content));
            task.Wait();
            var response = task.Result;

            var task2 = Task.Run(
             () => response.Content.ReadAsStreamAsync());
            task2.Wait();
            var contentStream = task2.Result;

            string tempDir = Path.Combine(Path.GetTempPath(), "WotRSpeechMod");
            
            // Create the temp directory if it doesn't exist
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            
            string guid = Guid.NewGuid().ToString();
            string fileName = $"audio_{guid}.wav";
            string outputPath = Path.Combine(tempDir, fileName);

            using FileStream stream = File.OpenWrite(outputPath);
            contentStream.CopyTo(stream);

            filesToPlay.Enqueue(outputPath);
        }



    }
}
