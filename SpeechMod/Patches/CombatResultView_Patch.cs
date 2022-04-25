using HarmonyLib;
using Kingmaker;
using Kingmaker.UI.MVVM._PCView.Crusade.CombatResult;
using SpeechMod.Unity;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch(typeof(CombatResultPCView), "BindViewImplementation")]
public static class CombatResultView_Patch
{
    public static void Postfix()
    {
        if (!Main.Enabled)
            return;

        var sceneName = Game.Instance.CurrentlyLoadedArea.ActiveUIScene.SceneName;

#if DEBUG
        Debug.Log($"{nameof(CombatResultPCView)}_BindViewImplementation_Postfix @ {sceneName}");
#endif

        var description = UIHelper.TryFindInStaticCanvas("CombatResultView/FoundDescription");
        if (description == null)
            Debug.LogWarning($"{nameof(description)} not found!");
        else
            description.HookupTextToSpeechOnTransform();

        var experience = UIHelper.TryFindInStaticCanvas("CombatResultView/Experience");
        if (experience == null)
            Debug.LogWarning($"{nameof(experience)} not found!");
        else
            experience.HookupTextToSpeechOnTransform();

        var resource = UIHelper.TryFindInStaticCanvas("CombatResultView/Resource");
        if (resource == null)
            Debug.LogWarning($"{nameof(resource)} not found!");
        else
            resource.HookupTextToSpeechOnTransform();
    }
}