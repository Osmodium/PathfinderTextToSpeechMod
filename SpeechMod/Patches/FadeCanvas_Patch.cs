using HarmonyLib;
using Kingmaker;
using Kingmaker.UI;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch(typeof(FadeCanvas), "Initialize")]
public class FadeCanvas_Patch
{
    public static void Postfix()
    {
        if (!Main.Enabled)
            return;

        var sceneName = Game.Instance.CurrentlyLoadedArea.ActiveUIScene.SceneName;

#if DEBUG
        Debug.Log($"{nameof(FadeCanvas)}_Initialize_Postfix @ {sceneName}");
#endif
        
        PlaybackControl.TryInstantiate();
    }
}