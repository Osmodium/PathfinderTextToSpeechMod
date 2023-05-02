using HarmonyLib;
using Kingmaker;
using Kingmaker.UI;
using Kingmaker.UI.MVVM._PCView.MainMenu;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch(typeof(MainMenuView), "Initialize")]
public class FadeCanvas_Patch
{
    public static void Postfix()
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(MainMenuView)}_Initialize_Postfix");
#endif

        PlaybackControl.TryInstantiate();
    }
}