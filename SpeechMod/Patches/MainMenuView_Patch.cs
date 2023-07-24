using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Localization;
using Kingmaker.UI.MVVM._PCView.MainMenu;
using UnityEngine;
using static Kingmaker.UI.KeyboardAccess;

namespace SpeechMod.Patches;

//[HarmonyPatch(typeof(MainMenuView), "Initialize")]
[HarmonyPatch(typeof(BlueprintsCache))]
public class MainMenuView_Patch
{
    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(nameof(BlueprintsCache.Init)), HarmonyPostfix]
    public static void Postfix()
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(MainMenuView)}_Initialize_Postfix");
#endif

        //PlaybackControl.TryInstantiate();

        //LocalizationManager.CurrentPack.PutString("speechmod.title", "SpeechMod");
        //LocalizationManager.CurrentPack.PutString("speechmod.stopplayback", "Stop play back");
        //LocalizationManager.CurrentPack.PutString("speechmod.stopplaybackdesc", "Erhm stops playback...");
        //var sb = SettingsBuilder.New("speechmod.hotkey.settings", new LocalizedString { Key = "speechmod.title" });
        //sb.AddKeyBinding(KeyBinding.New("speechmod.stopplayback", GameModesGroup.All, new LocalizedString { Key = "speechmod.stopplaybackdesc" }), Callback);
        //ModMenu.ModMenu.AddSettings(sb);
    }

    private static void Callback()
    {
        var s = "[SpeechMod] Keybind Callback!";
        Debug.Log(s);
        Main.Logger.Log(s);
    }
}