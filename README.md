# SpeechMod - Windows only
By [Osmodium](https://github.com/Osmodium)

## This mod is made for Pathfinder: Wrath of the Righteous and introduces TTS (TextToSpeech) to the dialog that is currently in focus and various text blocks when inspecting items (Info).
Version: 0.9.5

**Disclamer: for Windows version only**

### How to install

 1. Download and install Unity Mod Manager, make sure it is at least version 0.23.0 (I use 0.23.3)
 2. Run Unity Mod Manger and set it up to find Pathfinder: Wrath of the Righteous (Second Adventure)
 3. Download the SpeechMod-mod
 4. Install the mod by dragging the zip file from step 3 into the Unity Mod Manager window under the Mods tab. Alternatively locate the zip file after clicking the "Install" button in Unity Mod Manager.

### Known issues / limitations

*If you find issues or would like to request features, please use the issues tracker in GitHub [here](https://github.com/Osmodium/PathfinderTextToSpeechMod/issues)*

 - Only works on Windows, since it utilizes the built-in accessibility tool "SAPI" in Windows.
 - No stopping of voice implemented yet.
 - Pressing multiple times queues the text to be spoken after the current one has played through.
 - There'a currently a bug in the base game where sub-menus in the Encyclopedia disappears at times, this is not due to this mod.

### How to use

#### 1) Dialog 
When in dialog mode you can now press the play button next to the left image to listen to the current block of dialog.

![Playbutton for the current dialog](http://www.dashvoid.com/speechmod/playbutton_dialog.png)

#### 2) Book text
When inspecting a book (through *right-click->Info*) *hover* over the text and *left click*.

![Here the hover behaviour is set to underline the text, see the settings for more custumization](http://www.dashvoid.com/speechmod/booktext.png)

#### 3) Item text
When inspecting an item (through *right-click->Info*) *hover* over text (not all text is currently supported) and *left click*.

![Some of the texts are not supported yet. Try hovering different parts to see which are supported](http://www.dashvoid.com/speechmod/itemtext.png)

#### 4) Journal Quest text
In the journal, each of the bigger text blocks and important stuff can be played through the play button adjacent to the text.

![The most important parts of the journal text is supported.](http://www.dashvoid.com/speechmod/journaltext_0_9_5.png)

#### 5) Encyclopedia text
In the encyclopedia the text blocks (defined by Owlcat) can be played by pressing the play button adjacent to the text.

![All text parts in the encyclopedia is supported.](http://www.dashvoid.com/speechmod/encyclopediatext_0_9_5.png)

### Settings
The different settings (available through *ctrl+f10* if not overridden in the UMM) for SpeechMod
- **Speech rate**: The speed of the voice the higher number, the faster the speech. From -10 to 10, 1 is default.
- **Speech volume**: The volume of the voice from 0 to 100.
- **Speech pitch**: The pitch of the voice from -10 to 10.
- **Voice**: Select from the three Microsoft built-in voices available.
- **Enable color on hover**: This is used only for the text boxes when inspecting items, and colors the text the selected color when hovering the text box.
- **Enable font style on hover**: As above this is only used for text boxes, but lets you set the style of the font.
- **Phonetic Dictionary Reload**: Reloads the PhoneticDictionary.json into the game, to facilitate modificaton while playing.

![Settings for SpeechMod](http://www.dashvoid.com/speechmod/settings_0_9_5.png)

### Motivation
*Why did I create this mod?*
I have come to realize that I spend alot of my energy through the day on various activities, so when I get to play a game I rarely have enough energy left over to focus on reading long passages of text. So I thought it nice if I could get a helping hand so I wouldn't miss out on the excellent stories and writing in text heavy games.
After I started creating this mod, I have thought to myself that if I struggle with this issue, imageine what people with genuine disabilities must go through and possibly miss out on, which motivated me even more to get this mod working and release it. I really hope that it will help and encourage more people to get as much out of the game as possible.

### Contribute
If you find a name in the game which is pronounced funny by the voice, you can add it to the PhoneticDictionary.json in the mod folder (don't uninstall the mod as this will be deleted). I don't have a great way of submitting changes to this besides through GitHub pull requests, which is not super user friendly. But let's see if we can build a good pronounciation database for the voice together.

### Acknowledgments
- [Chad Weisshaar](https://chadweisshaar.com/blog/author/wp_admin/) for his blog about [Windows TTS for Unity](https://chadweisshaar.com/blog/2015/07/02/microsoft-speech-for-unity/)
- Pathfinder Wrath of The Righteous Discord channel members
- Join the [Discord](https://discord.gg/EFWq7rJFNN)
