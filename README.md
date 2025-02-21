# Pathfinder Elevenlabs Mod
Based on [SpeechMod](https://github.com/Osmodium/PathfinderTextToSpeechMod)

Uses the Elevenlabs API to fill in the gaps of the game's voice acting.

## Cost implications

Elevenlabs is not cheap. My best estimate is that a completionist playthrough of the game will cost around 100USD using the `eleven_flash_v2_5 ` and 200USD using the `eleven_multilingual_v2`.

Files are saved locally, so you can replay the game without additional cost (note that the saved audio will use the original characters name and gender).

## This mod is made for Pathfinder: Wrath of the Righteous and introduces TTS (TextToSpeech) in most places.

**Works with all languages supported by ElevenLabs.**

### How to install

 1. Download and install Unity Mod Manager, make sure it is at least version 0.23.0
 2. Run Unity Mod Manger and set it up to find Pathfinder: Wrath of the Righteous (Second Adventure)
 3. Download the latest release of the mod from the [releases page]()
 4. Install the mod by dragging the zip file from step 3 into the Unity Mod Manager window under the Mods tab. Alternatively locate the zip file after clicking the "Install" button in Unity Mod Manager.
 5. **IMPORTANT:** Fill in the `settings.json` file in the mod folder with the settings you want. (See below for more information)
  
 *If running on OSX 64-bit you might need to use the *mono console.exe* command (see UMM documentation for further)

### Known issues / limitations

*If you find issues or would like to request features, please use the issues tracker in GitHub [here](https://github.com/stevensewell/PathfinderElevenLabsMod/issues)*

#### Limitations:
 - No stopping of playback yet.
 - Volume is based on the system volume, not the game volume.

#### Issues todo:
  - No support for chapter changes (although they seem to be narrated).

### How to use

### 1) Settings

The `settings.json` file is used to configure voice assignments and API access. 

It is loaded once on game start, you'll need to restart the game to apply changes.

#### ApiKey (normally required)

`ApiKey`:

Your ElevenLabs API key. You can create an API key at https://elevenlabs.io/app/settings/api-keys.

If you've already completed a playthrough with this mod and would like to replay the game without incurring additional costs, you can set this to an empty string (`""`).

Example:

```json
"ApiKey" : "YOUR_API_KEY",
```

#### Narrator (required)

`Narrator`:

The voice ID to use for the narrator. This voice will be used for all narration in the game, and is the only required voice.

#### Model (required)

The elevenlabs model that should be used when sending text to the API. For more information on the models, see the [ElevenLabs API documentation](https://elevenlabs.io/docs/api-reference/text-to-speech).

Suggested models:

- `eleven_flash_v2_5` for lowest latency with moderate quality.
- `eleven_multilingual_v2` for high quality with a much longer response time and higher cost.

Example:

```json
"Model" : "eleven_flash_v2_5",
```

#### Generic Voices (optional)

`GenericMaleVoices`:

This array contains a list of [voice IDs](https://help.elevenlabs.io/hc/en-us/articles/14599760033937-How-do-I-find-my-voices-ID-of-my-voices-via-the-website-and-through-the-API). Male characters not explicitly listed in the `NamedCharacters` dictionary will deterministically use one of these voices.

Note that adding an additional voice during a playthrough will affect which voice is selected for a character.

Example:

```json
"GenericMaleVoices": ["VOICE_ID_1", "VOICE_ID_2", "ETC."],
```

`GenericFemaleVoices`:

As above, but for female voices.

#### Named Characters (optional)

`NamedCharacters`:

This dictionary is used to assign specific voices to named characters. The key is the character's name, and the value is the voice ID.

You can add any named character to the dictionary, and the mod will use the specified voice for that character. As a general rule the name that use in the dictionary will be the same as displayed in the game.
You can use partial names if the name is long: "inheritor" instead of "hand of the inheritor"

Example:

```json
"NamedCharacters" : {
  "arueshalae": "VOICE_ID",
  "woljif": "VOICE_ID",
}
```

#### Other settings (default recommended)

`SimilarityBoost`, `UseSpeakerBoost`, `Style`, `Stability`: values sent to the ElevenLabs API. For more information on these settings, see the [ElevenLabs API documentation](https://elevenlabs.io/docs/api-reference/text-to-speech).

`Enabled`: If set to false, the mod will not send any text to the API.

`SaveAudio`: Save the audio files to disk. If set to true, the mod will save the audio files in the  path specified by `AudioSavePath`.

`AudioSavePath`: Where to save the audio files. This path should be relative to the game root directory. This folder MUST exist before the mod is run.

`MinChars`: Dialogue with fewer characters than this value will not be sent to the API.


### Cloning voices

- Download [WwiseUnpacker](https://github.com/Vextil/Wwise-Unpacker/releases/tag/1.0.3)
- Unpack all voices in `Pathfinder Second Adventure\Wrath_Data\StreamingAssets\Audio\GeneratedSoundBanks\Windows\Packages`
- map file id to character using `Pathfinder Second Adventure\Wrath_Data\StreamingAssets\Audio\GeneratedSoundBanks\Windows\SoundbanksInfo.xml`
- Make an eleven labs instant clone using 2 or 3 voice samples
- Fill out each characters voice id (or delete the character and it will use a generic fall back)


#### 3) Dialog
When in dialog mode you can now press the play button next to the left image to listen to the current block of dialog. If autoplay is enabled, you don't have to push the playbutton.

![Playbutton for the current dialog](https://dashvoid.com/speechmod/wrath/playbutton_dialog.png)

#### 3) Book text
When inspecting a book (through *right-click->Info*) *hover* over the text and *left click*.

![Here the hover behaviour is set to underline the text, see the settings for more custumization](https://dashvoid.com/speechmod/wrath/booktext.png)

#### 4) Item text
When inspecting an item (through *right-click->Info*) *hover* over text (not all text is currently supported) and *left click*.

![Some of the texts are not supported yet. Try hovering different parts to see which are supported](https://dashvoid.com/speechmod/wrath/itemtext.png)

#### 5) Journal Quest text
In the journal, each of the bigger text blocks and important stuff can be played through the play button adjacent to the text.

![The most important parts of the journal text is supported.](https://dashvoid.com/speechmod/wrath/journaltext_0_9_5.png)

#### 6) Encyclopedia text
In the encyclopedia the text blocks (defined by Owlcat) can be played by pressing the play button adjacent to the text.

![All text parts in the encyclopedia are supported.](https://dashvoid.com/speechmod/wrath/encyclopediatext_0_9_5.png)

#### 7) Book Event text
When encountering a book event, the text can be played by hovering the text part (it will apply the chosen hover effect) and left-clicking.

![All text parts in a book event are supported. You might even get to know what the cut text says ;)](https://dashvoid.com/speechmod/wrath/eventbook_0_9_6.png)

#### 8) Messagebox text
The various pop-up boxes that eventually shows up throughout the game, can be played when hovered and left-clicked.

![Some texts might be so important that I decided to add support for them.](https://dashvoid.com/speechmod/wrath/messagemodal_0_9_6.png)


#### 9) Combat Results text
When your amy has defeated an enemy, the resulting message text is also supported for playback when hovered and left-clicked.

![Some of the combat results from armies might be important.](https://dashvoid.com/speechmod/wrath/combatresult_1_0_0.png)

#### 10) Tutorial Windows text
Both big and small tutorial windows text is supported and can be played by hovering and left-clicking.

![Tutorials can be helpful to learn more.](https://dashvoid.com/speechmod/wrath/tutorialsmall_1_0_0.png)

#### 11) Character story under summary and biography
When inspecting a character, the story of that character is displayed both under *Summary* and under *Biography*, and are both supported by hovering and left-clicking.

![Stories give companions depth.](https://dashvoid.com/speechmod/wrath/story_1_0_4.png)


### Acknowledgments
- [Osmodium](https://github.com/Osmodium) for creating the original mod.