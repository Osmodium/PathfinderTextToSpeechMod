using SpeechMod.Voice.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace SpeechMod.Voice
{
    public class KokoroSpeech : APISpeech
    {
        public KokoroSpeech() : base() { }

        protected override async Task ProcessAndQueueFile(string item, int count)
        {
            var jsonSettings = Main.JsonSettings;

            var reqItem = new KokoroRequestItem();
            reqItem.voice = jsonSettings.kokoro_settings.voice;
            reqItem.model = jsonSettings.kokoro_settings.model;
            reqItem.speed = jsonSettings.kokoro_settings.speed;
            reqItem.lang_code = jsonSettings.kokoro_settings.lang_code;
            reqItem.response_format = jsonSettings.kokoro_settings.response_format;

            var normalizationOptions = new NormalizationOptions();
            normalizationOptions.normalize = jsonSettings.kokoro_settings.normalize;
            normalizationOptions.unit_normalization = jsonSettings.kokoro_settings.unit_normalization;
            normalizationOptions.url_normalization = jsonSettings.kokoro_settings.url_normalization;
            normalizationOptions.email_normalization = jsonSettings.kokoro_settings.email_normalization;
            normalizationOptions.optional_pluralization_normalization = jsonSettings.kokoro_settings.optional_pluralization_normalization;

            reqItem.normalization_options = normalizationOptions;
            
            // non-configurable fields
            reqItem.input = item;
            reqItem.stream = true;
            reqItem.return_download_link = false;

            var content = JsonContent.Create(reqItem);

            var response = await sharedHttpClient.PostAsync(jsonSettings.endpoint, content);
            var contentStream = await response.Content.ReadAsStreamAsync();

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
            //return outputPath;
        }
    }
}
