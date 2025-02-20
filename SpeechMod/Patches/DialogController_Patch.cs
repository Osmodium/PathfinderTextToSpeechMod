using HarmonyLib;
using Kingmaker;
using Kingmaker.Controllers.Dialog;
using Kingmaker.DialogSystem.Blueprints;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch(typeof(DialogController), "SelectAnswer")]
public class DialogController_Patch
{
    public static void Prefix(BlueprintAnswer answer)
    {
        Main.WaveOutEvent?.Stop();
    }
}