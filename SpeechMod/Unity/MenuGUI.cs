using SpeechMod.Voice;
using System.Linq;
using UnityEngine;

namespace SpeechMod.Unity;

public static class MenuGUI
{
    private static string m_NarratorPreviewText = "Speech Mod for Pathfinder Wrath of the Righteous - Narrator voice speech test";
    private static string m_FemalePreviewText = "Speech Mod for Pathfinder Wrath of the Righteous - Female voice speech test";
    private static string m_MalePreviewText = "Speech Mod for Pathfinder Wrath of the Righteous - Male voice speech test";

    public static void OnGui()
    {
        AddVoiceSelector("Narrator Voice - See nationality below", ref Main.Settings.NarratorVoice, ref m_NarratorPreviewText, ref Main.Settings.NarratorRate, ref Main.Settings.NarratorVolume, ref Main.Settings.NarratorPitch, VoiceType.Narrator);

        GUILayout.BeginVertical("", GUI.skin.box);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Use gender specific voices", GUILayout.ExpandWidth(false));
        Main.Settings.UseGenderSpecificVoices = GUILayout.Toggle(Main.Settings.UseGenderSpecificVoices, "Enabled");
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        if (Main.Settings.UseGenderSpecificVoices)
        {
            AddVoiceSelector("Female Voice - See nationality below", ref Main.Settings.FemaleVoice, ref m_FemalePreviewText, ref Main.Settings.FemaleRate, ref Main.Settings.FemaleVolume, ref Main.Settings.FemalePitch, VoiceType.Female);
            AddVoiceSelector("Male Voice - See nationality below", ref Main.Settings.MaleVoice, ref m_MalePreviewText, ref Main.Settings.MaleRate, ref Main.Settings.MaleVolume, ref Main.Settings.MalePitch, VoiceType.Male);
        }

        GUILayout.BeginVertical("", GUI.skin.box);

        if (Main.Speech is WindowsSpeech)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Interrupt speech on play", GUILayout.ExpandWidth(false));
            GUILayout.Space(10);
            Main.Settings.InterruptPlaybackOnPlay = GUILayout.Toggle(Main.Settings.InterruptPlaybackOnPlay, Main.Settings.InterruptPlaybackOnPlay ? "Interrupt and play" : "Add to queue");
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Auto play dialog", GUILayout.ExpandWidth(false));
        GUILayout.Space(10);
        Main.Settings.AutoPlay = GUILayout.Toggle(Main.Settings.AutoPlay, "Enabled");
        GUILayout.EndHorizontal();

        {
            GUI.enabled = Main.Settings.AutoPlay;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Auto play ignores voiced dialog lines", GUILayout.ExpandWidth(false));
            GUILayout.Space(10);
            Main.Settings.AutoPlayIgnoreVoice = GUILayout.Toggle(Main.Settings.AutoPlayIgnoreVoice, "Enabled");
            GUILayout.EndHorizontal();

            GUI.enabled = true;
        }

        GUILayout.EndVertical();

        AddColorPicker("Color on text hover", ref Main.Settings.ColorOnHover, "Hover color", ref Main.Settings.HoverColorR, ref Main.Settings.HoverColorG, ref Main.Settings.HoverColorB, ref Main.Settings.HoverColorA);

        GUILayout.BeginVertical("", GUI.skin.box);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Font style on text hover", GUILayout.ExpandWidth(false));
        Main.Settings.FontStyleOnHover = GUILayout.Toggle(Main.Settings.FontStyleOnHover, "Enabled");
        GUILayout.EndHorizontal();

        if (Main.Settings.FontStyleOnHover)
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < Main.Settings.FontStyles.Length; ++i)
            {
                Main.Settings.FontStyles[i] = GUILayout.Toggle(Main.Settings.FontStyles[i], Main.FontStyleNames[i], GUILayout.ExpandWidth(true));
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();

        GUILayout.BeginVertical("", GUI.skin.box);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Phonetic dictionary", GUILayout.ExpandWidth(false));
        GUILayout.Space(10);
        if (GUILayout.Button("Reload", GUILayout.ExpandWidth(false)))
            SpeechExtensions.LoadDictionary();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private static void AddVoiceSelector(string label, ref int voice, ref string previewString, ref int rate, ref int volume, ref int pitch, VoiceType type)
    {
        GUILayout.BeginVertical("", GUI.skin.box);

        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        voice = GUILayout.SelectionGrid(voice, Main.VoicesDict.Select(v => new GUIContent(v.Key, v.Value)).ToArray(),
            Main.Speech is WindowsSpeech ? 4 : 5
        );
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Nationality", GUILayout.ExpandWidth(false));
        GUILayout.Space(10);
        GUILayout.Label(Main.VoicesDict.ElementAt(voice).Value, GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Speech rate", GUILayout.ExpandWidth(false));
        GUILayout.Space(10);
        rate = Main.Speech switch
        {
            WindowsSpeech => (int)GUILayout.HorizontalSlider(rate, -10, 10, GUILayout.Width(300f)),
            AppleSpeech => (int)GUILayout.HorizontalSlider(rate, 150, 300, GUILayout.Width(300f)),
            _ => rate
        };
        GUILayout.Label($" {rate}", GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        if (Main.Speech is WindowsSpeech)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Speech volume", GUILayout.ExpandWidth(false));
            GUILayout.Space(10);
            volume = (int)GUILayout.HorizontalSlider(volume, 0, 100, GUILayout.Width(300f));
            GUILayout.Label($" {volume}", GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Speech pitch", GUILayout.ExpandWidth(false));
            pitch = (int)GUILayout.HorizontalSlider(pitch, -10, 10, GUILayout.Width(300f));
            GUILayout.Label($" {pitch}", GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Preivew voice", GUILayout.ExpandWidth(false));
        GUILayout.Space(10);
        previewString = GUILayout.TextField(previewString, GUILayout.Width(700f));
        if (GUILayout.Button("Play", GUILayout.ExpandWidth(true)))
            Main.Speech.SpeakPreview(previewString, type);
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private static void AddColorPicker(string enableLabel, ref bool enabledBool, string colorLabel, ref float r, ref float g, ref float b, ref float a)
    {
        GUILayout.BeginVertical("", GUI.skin.box);
        GUILayout.BeginHorizontal();
        GUILayout.Label(enableLabel, GUILayout.ExpandWidth(false));
        enabledBool = GUILayout.Toggle(enabledBool, "Enabled");
        GUILayout.EndHorizontal();

        if (enabledBool)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(colorLabel, GUILayout.ExpandWidth(false));
            GUILayout.Space(10);
            GUILayout.Label("R ", GUILayout.ExpandWidth(false));
            r = GUILayout.HorizontalSlider(r, 0, 1);
            GUILayout.Space(10);
            GUILayout.Label("G", GUILayout.ExpandWidth(false));
            g = GUILayout.HorizontalSlider(g, 0, 1);
            GUILayout.Space(10);
            GUILayout.Label("B", GUILayout.ExpandWidth(false));
            b = GUILayout.HorizontalSlider(b, 0, 1);
            GUILayout.Space(10);
            GUILayout.Label("A", GUILayout.ExpandWidth(false));
            a = GUILayout.HorizontalSlider(a, 0, 1);
            GUILayout.Space(10);
            GUILayout.Box(GetColorPreview(ref r, ref g, ref b, ref a), GUILayout.Width(20));
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    private static Texture2D GetColorPreview(ref float r, ref float g, ref float b, ref float a)
    {
        var texture = new Texture2D(20, 20);
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, new Color(r, g, b, a));
            }
        }
        texture.Apply();
        return texture;
    }

    public static void UpdateColors()
    {
        UIHelper.UpdateHoverColor();
    }
}