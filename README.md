# WotR-API-TextToSpeechMod
By [lvaskys](https://github.com/lvaskys)
Fork of [PathfinderTextToSpeechMod](https://github.com/Osmodium/PathfinderTextToSpeechMod)

This is based on [PathfinderTextToSpeechMod](https://github.com/Osmodium/PathfinderTextToSpeechMod) and currently preserves all its functionality and adds the ability to use a backend API for TTS instead of the Windows TTS engine. Currently, [Auralis](https://github.com/astramind-ai/Auralis) (based on xttsv2) and [Kokoro-FastAPI](https://github.com/remsky/Kokoro-FastAPI) are supported. I believe an NVIDIA gpu is required for both, but see their documentation for more information.

See [SpeechMod-README.md](SpeechMod-README.md) for the original README.md on how to install windows tts natural voices (if desired) and the basic functionality of the mod.

## How to use

This mod's main features are configured in a `settings.json` file that lives in the base mod folder. Comments are included to help guide your configuration. Of note is `speech_impl` which defines the implementation of the speech service to use, either `AuralisSpeech` or `KokoroSpeech` (this new implementation) or `WindowsSpeech` or `AppleSpeech` for the original implementation.

The API service must be up and running for the mod to work. See the documentation for the API service you are using for more information on how to set it up. I used WSL to run Auralis, although it may work in native Windows now as well, I'm not sure. For Kokoro, I used the [docker-run](https://github.com/remsky/Kokoro-FastAPI?tab=readme-ov-file#get-started) instructions. I think docker on Windows may require WSL for proper sharing of gpu to the container, so you may need to install it either way.

Make sure the endpoint matches. If you are keeping the `settings.json` file as is, then for Auralis:
```
auralis.openai --host 127.0.0.1 --port 8000 --model AstraMindAI/xttsv2 --gpt_model AstraMindAI/xtts2-gpt --max_concurrency 4 --vllm_logging_level warn
```
or Kokoro:
```
docker run --gpus all -p 8000:8880 ghcr.io/remsky/kokoro-fastapi-gpu:v0.2.2
```

**Note: in order to use Auralis, you must provide a wav file for server to use for one-shot voice cloning**
Currently, this is set up to live in your base game directory, not your mod directory. Although perhaps that can be fixed in the future. An example file you can use is [female_01.wav](samples/female_01.wav).

## Other new features
This supports cancelling playback with the controller cancel/B/Circle button. Specifically, it will cancel the current sentence or two sentence chunk being played and continue with the next sentence. This allows for a kind of "fast-forward" type effect if you don't feel like listening to the entire dialogue, but still want to hear later portions. Like, for example, if your reading outpaces the speaker.

## Limitations/Broken Features
These new features only support one speaker at the moment. Both voiced and narrator content will be spoken with the chosen voice. I may fully implement male/female/narrator as it was in the original mod, or maybe even characters-specific voices. But as it stands for now, this is a good initial release and works fine for my own needs.

## Motivation and thoughts
Windows natural TTS voices are pretty good, but lack proper cadence and emotion. XTTS is excellent in that regard, and seems to pick up on cues without even feeding it any additional information. The sound quality is poorer, however, and it is a good bit slower, but still responsive enough for my needs. Kororo is another TTS I heard about, and decided to add it as an option as well for another alternative. It's super fast, many times more than realtime, and the quality is excellent. The cadence and emotion aren't super, though, and seem rather similar to Windows natural voices.