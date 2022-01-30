# SpeechMod
By [Osmodium](https://github.com/Osmodium)

## This mod is made for Pathfinder: Wrath of the Righteous and introduces TTS (TextToSpeech) in most places.
Version: 1.0.0

**Disclaimer: Works on Windows and macOS only**

### How to install

 1. Download and install Unity Mod Manager, make sure it is at least version 0.23.0 (I use 0.23.3)
 2. Run Unity Mod Manger and set it up to find Pathfinder: Wrath of the Righteous (Second Adventure)
 3. Download the SpeechMod-mod
 4. Install the mod by dragging the zip file from step 3 into the Unity Mod Manager window under the Mods tab. Alternatively locate the zip file after clicking the "Install" button in Unity Mod Manager.
 
 *If running on OSX 64-bit you might need to use the *mono console.exe* command (see UMM documentation for further)

### Known issues / limitations

*If you find issues or would like to request features, please use the issues tracker in GitHub [here](https://github.com/Osmodium/PathfinderTextToSpeechMod/issues)*

#### Limitations:
 - There's currently a bug in the base game where sub-menus in the Encyclopedia disappears at times, this is not due to this mod.

#### Issues todo:
 - No control to stop playback manually (want to do a visualization at some point).
 - No support for chapter changes (although they seem to be narrated).

### How to use

#### 1) Dialog 
When in dialog mode you can now press the play button next to the left image to listen to the current block of dialog. If autoplay is enabled, you don't have to push the playbutton.

![Playbutton for the current dialog](http://dashvoid.com/speechmod/playbutton_dialog.png)

#### 2) Book text
When inspecting a book (through *right-click->Info*) *hover* over the text and *left click*.

![Here the hover behaviour is set to underline the text, see the settings for more custumization](http://dashvoid.com/speechmod/booktext.png)

#### 3) Item text
When inspecting an item (through *right-click->Info*) *hover* over text (not all text is currently supported) and *left click*.

![Some of the texts are not supported yet. Try hovering different parts to see which are supported](http://dashvoid.com/speechmod/itemtext.png)

#### 4) Journal Quest text
In the journal, each of the bigger text blocks and important stuff can be played through the play button adjacent to the text.

![The most important parts of the journal text is supported.](http://dashvoid.com/speechmod/journaltext_0_9_5.png)

#### 5) Encyclopedia text
In the encyclopedia the text blocks (defined by Owlcat) can be played by pressing the play button adjacent to the text.

![All text parts in the encyclopedia is supported.](http://dashvoid.com/speechmod/encyclopediatext_0_9_5.png)

#### 6) Book Event text
When encountering a book event, the text can be played by hovering the text part (it will apply the chosen hover effect) and left-clicking.

![All text parts in a book event is supported. You might even get to know what the cut text says ;)](http://dashvoid.com/speechmod/eventbook_0_9_6.png)

#### 7) Messagebox text
The various pop-up boxes that eventually shows up throughout the game, can be played when hovered and left-clicked.

![Some texts might be so important that I decided to add support for them.](http://dashvoid.com/speechmod/messagemodal_0_9_6.png)


#### 8) Combat Results text
When your amy has defeated an enemy, the resulting message text is also supported for playback when hovered and left-clicked.

![Some of the combat results from armies might be important.](http://dashvoid.com/speechmod/combatresult_1_0_0.png)

#### 9) Tutorial Windows
Both big and small tutorial windows text is supported and can be played by hovering and left-clicking.

![Tutorials can be helpful to learn more.](http://dashvoid.com/speechmod/tutorialsmall_1_0_0.png)

### Settings

The different settings (available through *ctrl+f10* if not overridden in the UMM) for SpeechMod
- **Narrator Voice**: The settings for the voice used for either all or non-gender specific text in dialogs when *Use gender specific voices* is turned on.
	- *Nationality*: Just shows the selected voices nationality.
	- **Speech rate**: The speed of the voice the higher number, the faster the speech. 
		- Windows: from -10 to 10 (relative speed from 0). 
		- macOS: from 150 to 300 (words per minute).
	- Windows Only:
		- **Speech volume**: The volume of the voice from 0 to 100.
		- **Speech pitch**: The pitch of the voice from -10 to 10.
	-**Preview Voice**: Used to preview the settings of the voice.
- **Use gender specific voices**: Specify voices for female and male dialog parts. Each of the voices can be adjusted with rate, volume and pitch where available.
- Windows Only:
	- **Interrupt speech on play**: 2 settings: *Interrupt and play* or *Add to queue*, hope this speaks for itself.
- **Auto play dialog**: When enabled, dialogs will be played automatically when theres no voice acted dialog.
- **Enable color on hover**: This is used only for the text boxes when inspecting items, and colors the text the selected color when hovering the text box.
- **Enable font style on hover**: As above this is only used for text boxes, but lets you set the style of the font.
- **Phonetic Dictionary Reload**: Reloads the PhoneticDictionary.json into the game, to facilitate modificaton while playing.

![Settings for SpeechMod](http://dashvoid.com/speechmod/settings_1_0_0.png)

### Motivation
*Why did I create this mod?*
I have come to realize that I spend alot of my energy through the day on various activities, so when I get to play a game I rarely have enough energy left over to focus on reading long passages of text. So I thought it nice if I could get a helping hand so I wouldn't miss out on the excellent stories and writing in text heavy games.
After I started creating this mod, I have thought to myself that if I struggle with this issue, imageine what people with genuine disabilities must go through and possibly miss out on, which motivated me even more to get this mod working and release it. I really hope that it will help and encourage more people to get as much out of the game as possible.

### Contribute
If you find a name in the game which is pronounced funny by the voice, you can add it to the PhoneticDictionary.json in the mod folder (don't uninstall the mod as this will be deleted). I don't have a great way of submitting changes to this besides through GitHub pull requests, which is not super user friendly. But let's see if we can build a good pronounciation database for the voice together.
Also feel free to hit me up with ideas, issues and PRs on GitHub or NexusMods :)

### Acknowledgments
- [Chad Weisshaar](https://chadweisshaar.com/blog/author/wp_admin/) for his blog about [Windows TTS for Unity](https://chadweisshaar.com/blog/2015/07/02/microsoft-speech-for-unity/)
- [dope0ne](https://forums.nexusmods.com/index.php?/user/895998-dope0ne/) (zer0bits) for providing code to support macOS, and various exploration work.
- Pathfinder Wrath of The Righteous Discord channel members
- Join the [Discord](https://discord.gg/EFWq7rJFNN)