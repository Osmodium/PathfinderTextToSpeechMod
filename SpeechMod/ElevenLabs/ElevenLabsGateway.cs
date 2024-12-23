using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace SpeechMod.ElevenLabs;

public class ElevenReq
{
    public string Text { get; set; }
    public string ModelID { get; set; }
    public string Voice { get; set; }
}

public static class ElevenLabsGateway
{
    public static async Task<Stream> CreateStream(ElevenReq req)
    {
        try
        {
            Debug.Log("Http Client");
            using var client = new HttpClient();

            Debug.Log("Json");
            var json = new
            {
                text = req.Text,
                model_id = req.ModelID,
                output_format = "mp3_22050_32",
                optimize_streaming_latency = 3,
                voice_settings = new
                {
                    stability = Main.VoiceSettings.Stability,
                    style = Main.VoiceSettings.Style,
                    use_speaker_boost = Main.VoiceSettings.UseSpeakerBoost,
                    similarity_boost = Main.VoiceSettings.SimilarityBoost,
                }
            };

            Debug.Log(JsonConvert.SerializeObject(json));

            Debug.Log("Headers");
            client.DefaultRequestHeaders.Add("xi-api-key", Main.VoiceSettings.ApiKey);

            var content = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"https://api.elevenlabs.io/v1/text-to-speech/{req.Voice}/stream", content);

            Debug.Log("request sent");

            response.EnsureSuccessStatusCode();
            var responseStream = await response.Content.ReadAsStreamAsync();

            return responseStream;
        }
        catch (Exception e)
        {
            Debug.Log("error playing stream");
            Debug.Log(e.Message);
        }

        return null;
    }
}