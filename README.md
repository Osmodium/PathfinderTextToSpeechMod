  

# SpeechMod - Windows only
By [Osmodium](https://github.com/Osmodium)

## This mod is made for Pathfinder: Wrath of the Righteous and introduces TTS (TextToSpeech) to the dialog that is currently in focus and various text blocks when inspecting items (Info).

**Disclamer: for Windows version only**

### How to install

 1. Download and install Unity Mod Manager, make sure it is at least version 0.23.0 (I use 0.23.3)
 2. Run Unity Mod Manger and set it up to find Pathfinder: Wrath of the Righteous (Second Adventure)
 3. Download the SpeechMod mod
 4. Install the mod by dragging the zip file from step 3 into the Unity Mod Manager window under the Mods tab

### Issues

 - Only works on Windows!
 - No stopping of voice implemented yet.
 - Pressing multiple times queues the text to spoken after the current one has played through.
 - "Hardcoded" voices to the three old voices, since we are using SAPI voices.

### How to use

#### 1) Dialog 
When in dialog mode you can now press the play button next to the left image to listen to the current block of dialog.

![Playbutton for the current dialog](http://www.dashvoid.com/speechmod/playbutton_dialog.png)

#### 2) Book text
When inspecting a book (through right-click->Info) hover over the text and left click.

![Here the hover behaviour is set to underline the text, see the settings for more custumization](http://www.dashvoid.com/speechmod/booktext.png)

#### 3) Item text
When inspecting an item (through right-click->Info) hover over text (not all text is currently supported) and left click.

![Some of the texts are not supported yet. Try hovering different parts to see which are supported](http://www.dashvoid.com/speechmod/itemtext.png)

### Settings
The different settings (available through ctrl+f10 if not overridden in the UMM) for SpeechMod
**Speech rate**: The speed of the voice the higher number, the faster speech. 1 is default.
**Speech volume**: The volume of the voice from 0 to 100.
**Voice**: Select from the three Microsoft built-in voices (currently hardcoded, so if you haven't installed the voice, no voice will be heard).
**Enable color on hover**: This is used only for the text boxes when inspecting items, and colors the text the selected color when hovering the text box.
**Enable font style on hover**: As above this is only used for text boxes, but lets you set the style of the font.

![Settings for SpeechMod](http://www.dashvoid.com/speechmod/settings.png)

### Acknowledgments
- [Chad Weisshaar](https://chadweisshaar.com/blog/author/wp_admin/) for his blog about [Windows TTS for Unity](https://chadweisshaar.com/blog/2015/07/02/microsoft-speech-for-unity/)
- Pathfinder Wrath of The Righteous Discord channel members
- Join the [Discord](https://discord.gg/EFWq7rJFNN)
