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

namespace SpeechMod.Voice
{
    public class AuralisSpeech : ISpeech
    {

        private static HttpClient sharedHttpClient = new()
        {
            BaseAddress = new Uri("http://localhost:8000"),
        };

        private static int extraNum = 1;
        private ConcurrentQueue<string> filesToPlay = new ConcurrentQueue<string>();
        private volatile bool isPlaying = false;
        private IWavePlayer wavePlayer;
        private WaveStream waveStream;

        public AuralisSpeech()
        {
            Task.Run(() =>
            {
                doPlayback();
            });
        }

        // an infinite loop that will keep checking both if isPlaying and the concurrentqueue
        // if it's not playing, and there is stuff in the queue, then flag isPlaying, pop, 
        // wait until done, then keep on trucking....

        private void doPlayback()
        {

            while (true)
            {
                // do I even need a semaphore in this instance?....
                //await gate.WaitAsync();


                if (!isPlaying)
                {
                    if (filesToPlay.TryDequeue(out var fileToPlay))
                    {
                        isPlaying = true;
                        UnityEngine.Debug.Log("File path: " + fileToPlay);
                        PlayFile(fileToPlay);
                        isPlaying = false;
                    }
                }


                //gate.Release();

                // sleep for 5 seconds, then print log file
                //Thread.Sleep(5000);

                //UnityEngine.Debug.Log("In INFINITE LOOP worker thread, 5 seconds");

            }

        }

        private void PlayFile(string filePath)
        {
            try
            {
                string path = Path.GetFullPath(filePath);
                Main.Logger?.Log("Playing audio file using NAudio");
                Main.Logger?.Log(path);
                
                // Dispose previous player and stream if they exist
                DisposeWaveObjects();
                
                // Create a new WaveOut device
                wavePlayer = new WaveOutEvent();
                
                // Create a new AudioFileReader for the wave file
                waveStream = new AudioFileReader(path);
                
                // Set up event handling for when playback is finished
                wavePlayer.PlaybackStopped += (sender, args) =>
                {
                    Main.Logger?.Log("Playback finished");
                    DisposeWaveObjects();
                    DeleteFile(filePath);
                };
                
                // Connect the reader to the player and start playback
                wavePlayer.Init(waveStream);
                wavePlayer.Play();
                
                // Wait until playback is complete
                while (wavePlayer != null && wavePlayer.PlaybackState == PlaybackState.Playing)
                {
                    System.Threading.Thread.Sleep(100);
                }
                
                Main.Logger?.Log("Done playing");
            }
            catch (Exception ex)
            {
                Main.Logger?.Log($"Error playing audio: {ex.Message}");
                DisposeWaveObjects();
                DeleteFile(filePath);
            }
        }
        
        private void DisposeWaveObjects()
        {
            if (wavePlayer != null)
            {
                wavePlayer.Stop();
                wavePlayer.Dispose();
                wavePlayer = null;
            }
            
            if (waveStream != null)
            {
                waveStream.Dispose();
                waveStream = null;
            }
        }

        private void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }


        // TODO make this configurable...right now just return the one voice we have set up, "female_01.wav"
        public string[] GetAvailableVoices()
        {
            return new string[] { "female_01.wav" };
        }

        // TODO check what this is for....why do we need this?
        public string GetStatusMessage()
        {
            if (isPlaying)
            {
                return "Speaking";
            }
            else
            {
                return "Ready";
            }
        }

        public bool IsSpeaking()
        {
            return isPlaying || !filesToPlay.IsEmpty;
        }

        // TODO actually implement delay?
        public void Speak(string text, float delay = 0)
        {
            if (string.IsNullOrEmpty(text))
            {
                Main.Logger?.Warning("No text to speak!");
                return;
            }

            text = PrepareSpeechText(text);

        }

        public void SpeakAs(string text, VoiceType voiceType, float delay = 0)
        {
            if (string.IsNullOrEmpty(text))
            {
                Main.Logger?.Warning("No text to speak!");
                return;
            }

            // TODO maybe implement gender specific/nararrator voices later
            //if (!Main.Settings.UseGenderSpecificVoices)
            //{

            //}

            Speak(text, delay);
            return;
        }

        public void SpeakDialog(string text, float delay = 0)
        {
            if (string.IsNullOrEmpty(text))
            {
                Main.Logger?.Warning("No text to speak!");
                return;
            }

            // TODO maybe implement gender specific/nararrator voices later
            //if (!Main.Settings.UseGenderSpecificVoices)
            //{

            //}

            Speak(text, delay);
            return;
        }

        public void SpeakPreview(string text, VoiceType voiceType)
        {
            if (string.IsNullOrEmpty(text))
            {
                Main.Logger?.Warning("No text to speak!");
                return;
            }

            // TODO maybe implement gender specific/nararrator voices later
            //if (!Main.Settings.UseGenderSpecificVoices)
            //{

            //}

            Speak(text, 0);
            return;
        }

        public void Stop()
        {
            // Stop NAudio playback
            DisposeWaveObjects();
            
            // Clear the queue
            while (filesToPlay.TryDequeue(out _)) { }
            
            isPlaying = false;
        }

        private void ProcessAndQueueFile(string item, int count)
        {
            // Path to the .wav file in the base game directory
            // TODO: this should be configurable
            string filePath = "female_01.wav";
            byte[] fileBytes = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(fileBytes);

            var reqItem = new RequestItem();
            reqItem.voice = new string[] { base64String };
            reqItem.response_format = "wav";
            reqItem.input = item;
            reqItem.speed = 1.0f;
            reqItem.model = "xttsv2";

            reqItem.enhance_speech = true;
            reqItem.sound_norm_refs = false;
            reqItem.max_ref_length = 60;
            reqItem.gpt_cond_len = 30;
            reqItem.gpt_cond_chunk_len = 4;
            reqItem.temperature = 0.75f;
            reqItem.top_p = 0.85f;
            reqItem.top_k = 50f;
            reqItem.repetition_penalty = 5.0f;
            reqItem.length_penalty = 1.0f;
            reqItem.do_sample = true;
            reqItem.language = "auto";

            var content = JsonContent.Create(reqItem);

            // since this already runs in a seperate thread, these calls to the TTS service will run synchronously
            var task = Task.Run(() => sharedHttpClient.PostAsync("http://127.0.0.1:8000/v1/audio/speech", content));
            task.Wait();
            var response = task.Result;

            var task2 = Task.Run(
             () => response.Content.ReadAsStreamAsync());
            task2.Wait();
            var contentStream = task2.Result;

            extraNum++;
            string tempDir = Path.Combine(Path.GetTempPath(), "AuralisSpeech");
            
            // Create the temp directory if it doesn't exist
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            
            string fileName = $"file{count}-{extraNum}.wav";
            string outputPath = Path.Combine(tempDir, fileName);

            using FileStream stream = File.OpenWrite(outputPath);
            contentStream.CopyTo(stream);

            //Main.Logger?.Log("wrote wav");

            filesToPlay.Enqueue(outputPath);

            //Main.Logger?.Log("FINISHED processAndQueueFile");
        }


        // TODO add serction to remove nararator, or later to split it into a seperate voice
        // TODO how do we know what's voiced yet?  (I think thatgets handled elsewhere...if we're here, it needs ai voice)
        public string PrepareSpeechText(string text)
        {
            Main.Logger?.Warning("TESTTTTTTTTTESSSTTTTT " + text);
#if DEBUG
            Main.Logger?.Log("Enter 1");
            UnityEngine.Debug.Log("Init text: " + text);
#endif
            string[] textArr;
            // getting rid of all tags?
            // TODO might not need to anymore, I think they;re never added anymore
            text = new Regex("<[^>]+>").Replace(text, "");
            text = text.PrepareText();
            // separate each new line into a separate string then add to an array of strings called textArray
            textArr = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            UnityEngine.Debug.Log("After adding stuff: " + text);
            UnityEngine.Debug.Log("Text length: " + textArr.Length);

            string pattern = @"(?<=\/>|>)([^<]+)";
            Regex regex = new Regex(pattern);


            textArr = textArr.Select(item => Regex.Replace(item, @"<silence(?:\s+msec=""(\d+)"")?\/>", match => match.Groups[1].Success ? "..." : "")).ToArray();


            // each line split up into more chunks, count up to 20 words, then round up to the 
            // nearest sentence

            List<string> phrases = new List<string>();
            foreach (string line in textArr)
            {
                string phrase = "";
                string[] splitBySpaces = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                int wordCount = 0;
                for (int i = 0; i < splitBySpaces.Length; i++)
                {
                    wordCount++;
                    phrase += " " + splitBySpaces[i];

                    if (wordCount >= 20 && (
                        splitBySpaces[i].EndsWith(".") ||
                        splitBySpaces[i].EndsWith("?") ||
                        splitBySpaces[i].EndsWith("!")
                        ))
                    {
                        phrases.Add(phrase);
                        wordCount = 0;
                        phrase = "";
                    }
                }

                // add the lst phrase if didn't end in period
                // and check if not empty
                if (!string.IsNullOrWhiteSpace(phrase))
                {
                    phrases.Add(phrase);
                }
            }

            
            Task.Run(() =>
            {
                ProcessText(phrases.ToArray());
            });
#if DEBUG
            if (System.Reflection.Assembly.GetEntryAssembly() == null)
                Main.Logger?.Warning("Invalid " + text);
            UnityEngine.Debug.Log(text);
#endif
            return text;
        }

        private void ProcessText(string[] text)
        {
           
            //Main.Logger?.Warning("INSIDE ProcessText!");
            //UnityEngine.Debug.Log("------size of queue: " + filesToPlay.Count);

            while (filesToPlay.TryPeek(out var fileToPlay))
            {
                if (filesToPlay.TryDequeue(out var fileToPlay2))
                {
                    // clear list
                }

            }
            //UnityEngine.Debug.Log("------size of queue after clear: " + filesToPlay.Count);
            
            Stop();

            int count = 0;
            foreach (var item in text)
            {
                ProcessAndQueueFile(item, count++);
            }

        }


    }
}
