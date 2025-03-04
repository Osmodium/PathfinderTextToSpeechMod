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
    public abstract class APISpeech : ISpeech
    {
        public static HttpClient sharedHttpClient = new();
        protected ConcurrentQueue<string> filesToPlay = new();
        public volatile bool isPlaying = false;
        private IWavePlayer wavePlayer;
        private WaveStream waveStream;
        private EventHandler<StoppedEventArgs> playbackStoppedHandler;


        public APISpeech()
        {
            Task.Run(() =>
            {
                doPlayback();
            });
        }

        // an infinite loop that will keep checking both if isPlaying and the concurrentqueue
        // if it's not playing, and there is stuff in the queue, then flag isPlaying, pop, 
        // wait until done, then continue...

        private void doPlayback()
        {

            while (true)
            {

                if (!isPlaying)
                {
                    if (filesToPlay.TryDequeue(out var fileToPlay))
                    {
                        isPlaying = true;
                        //UnityEngine.Debug.Log("File path: " + fileToPlay);
                        PlayFile(fileToPlay);
                        isPlaying = false;
                    }

                    Thread.Sleep(100);
                }
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
                playbackStoppedHandler= (sender, args) =>
                {
                    Main.Logger?.Log("Playback finished");
                    DisposeWaveObjects();
                    DeleteFile(filePath);
                };

                wavePlayer.PlaybackStopped += playbackStoppedHandler;
                
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
                if (playbackStoppedHandler != null)
                {
                    wavePlayer.PlaybackStopped -= playbackStoppedHandler;
                }

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


        // TODO maybe use UMM config for this, but for now is using settings.json
        public string[] GetAvailableVoices()
        {
            return new string[] { "APIVoice" };
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

        // TODO actually implement delay? what is it used for?
        public void Speak(string text, float delay = 0)
        {
            if (string.IsNullOrEmpty(text))
            {
                Main.Logger?.Warning("No text to speak!");
                return;
            }

            PrepareSpeechText(text);
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
            isPlaying = true;
            StopWavePlayer();

            while (filesToPlay.TryPeek(out _))
            {
                if (filesToPlay.TryDequeue(out var fileToRemove))
                {
                    DeleteFile(fileToRemove);
                }
            }

            isPlaying = false;
        }

        private void StopWavePlayer()
        {
            if (wavePlayer != null && waveStream != null)
            {
                Main.Logger?.Log("Stopping audio playback");

                // Trigger the same handler that would occur naturally when playback stops
                // This will ensure the file is deleted and resources are disposed
                if (playbackStoppedHandler != null)
                {
                    playbackStoppedHandler(wavePlayer, new StoppedEventArgs());
                }
                else
                {
                    // Fallback in case the handler is not set
                    DisposeWaveObjects();
                }
            }
        }

        public void NextPhrase()
        {
            // make sure no new playback starts
            // TODO maybe a lock would be better?
            isPlaying = true;
            StopWavePlayer();
            isPlaying = false;
        }

        protected abstract void ProcessAndQueueFile(string item, int count);
       


        // TODO add serction to remove nararator, or later to split it into a seperate voice
        // TODO how do we know what's voiced yet?  (I think thatgets handled elsewhere...if we're here, it needs ai voice)
        // TODO FIXME could use some cleanup
        public string PrepareSpeechText(string text)
        {
#if DEBUG
            Main.Logger?.Log("PrepareSpeechText: " + text);
#endif
            string[] textArr;
            // getting rid of all tags?
            // TODO might not need to anymore, I think they;re never added anymore
            text = new Regex("<[^>]+>").Replace(text, "");
            text = text.PrepareText();
            // separate each new line into a separate string then add to an array of strings called textArray
            // TODO this should never happen I think. FIXME later
            textArr = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            //UnityEngine.Debug.Log("After adding stuff: " + text);
            //UnityEngine.Debug.Log("Text length: " + textArr.Length);

            string pattern = @"(?<=\/>|>)([^<]+)";
            Regex regex = new Regex(pattern);

            textArr = textArr.Select(item => Regex.Replace(item, @"<silence(?:\s+msec=""(\d+)"")?\/>", match => match.Groups[1].Success ? "..." : "")).ToArray();


            // each line split up into more chunks, count up to 20 words, then round up to the 
            // nearest sentence

            // chunking like this makes more sense than trying to stream from the tts service
            // because the quality of the output is much better when chunked in at least a few
            // sentences rather than streaming. I think it might have to do with how much context
            // the tts service has with a full sentence or two vs maybe only a few words at a time
            // with streaming

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
#endif
            return text;
        }

        private void ProcessText(string[] text)
        {
            // this stop call may be redundant
            Stop();

            int count = 0;
            foreach (var item in text)
            {
                ProcessAndQueueFile(item, count++);
            }

        }


    }
}
