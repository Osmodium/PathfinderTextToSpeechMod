using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Kingmaker.Localization;
using Kingmaker.UI.MVVM._PCView.Settings.Menu;
using Kingmaker.UI.MVVM._VM.Settings;
using Kingmaker.UI.SettingsUI;
using Kingmaker.Utility;

namespace SpeechMod.External.ModMenu.Settings;

/// <summary>
/// Class containing patches necessary to inject an additional settings screen into the menu.
/// </summary>
internal class ModsMenuEntity
{
    // Random magic number representing our fake enum for UiSettingsManager.SettingsScreen
    private const int SettingsScreenValue = 17;
    internal static readonly UISettingsManager.SettingsScreen SettingsScreenId =
        (UISettingsManager.SettingsScreen)SettingsScreenValue;

    private static LocalizedString _menuTitleString;
    private static LocalizedString MenuTitleString
    {
        get
        {
            _menuTitleString ??= Helpers.CreateString(
                "ModsMenu.Title", "Mods", ruRU: "Моды");
            return _menuTitleString;
        }
    }

    private static readonly List<UISettingsGroup> ModSettings = new();

    internal static void Add(UISettingsGroup uiSettingsGroup)
    {
        ModSettings.Add(uiSettingsGroup);
    }

    /// <summary>
    /// Patch to create the Mods Menu ViewModel.
    /// </summary>
    [HarmonyPatch]
    static class SettingsVM_Constructor
    {
        static MethodBase TargetMethod()
        {
            // There's only a single constructor so grab the first one and ignore the arguments. Maybe I'll try adding
            // back the args version later but right now this works.
            return AccessTools.FirstConstructor(typeof(SettingsVM), c => true);
        }

        private static readonly MethodInfo CreateMenuEntity =
            AccessTools.Method(typeof(SettingsVM), nameof(SettingsVM.CreateMenuEntity));
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            // Look for the last usage of CreateMenuEntity, we want to insert just after that.
            var insertionIndex = 0;
            for (int i = code.Count - 1; i > 0; i--)
            {
                if (code[i].Calls(CreateMenuEntity))
                {
                    insertionIndex = i + 1; // increment since inserting at i would actually be before the insertion point
                    break;
                }
            }

            var newCode =
                new List<CodeInstruction>()
                {
                    new CodeInstruction(OpCodes.Ldarg_0), // Loads this
                    CodeInstruction.Call(typeof(SettingsVM_Constructor), nameof(SettingsVM_Constructor.AddMenuEntity)),
                };

            code.InsertRange(insertionIndex, newCode);
            return code;
        }

        private static void AddMenuEntity(SettingsVM settings)
        {
            try
            {
                settings.CreateMenuEntity(MenuTitleString, SettingsScreenId);
                Main.Logger.NativeLog("Added Mods Menu ViewModel.");
            }
            catch (Exception e)
            {
                Main.Logger.LogException(e);
            }
        }
    }

    /// <summary>
    /// Patch to create the Mods Menu View. Needed to show the menu in-game.
    /// </summary>
    [HarmonyPatch(typeof(SettingsMenuSelectorPCView))]
    static class SettingsMenuSelectorPCView_Patch
    {
        [HarmonyPatch(nameof(SettingsMenuSelectorPCView.Initialize)), HarmonyPrefix]
        static void Initialize_Prefix(SettingsMenuSelectorPCView __instance)
        {
            try
            {
                if (!__instance.m_MenuEntities.Any())
                {
                    __instance.m_MenuEntities = new List<SettingsMenuEntityPCView>();
                    __instance.m_MenuEntities.AddRange(__instance.GetComponentsInChildren<SettingsMenuEntityPCView>());
                    var existingEntity = __instance.m_MenuEntities.LastItem();
                    var newEntity = UnityEngine.Object.Instantiate(existingEntity);
                    newEntity.transform.SetParent(existingEntity.transform.parent);
                    __instance.m_MenuEntities.Add(newEntity);
                    Main.Logger.NativeLog("Added Mods Menu View");
                }
            }
            catch (Exception e)
            {
                Main.Logger.LogException(e);
            }
        }
    }

    /// <summary>
    /// Patch to return the Mods settings list
    /// </summary>
    [HarmonyPatch(typeof(UISettingsManager))]
    static class UISettingsManager_GetSettingsList
    {
        [HarmonyPatch(nameof(UISettingsManager.GetSettingsList)), HarmonyPostfix]
        static void Postfix(UISettingsManager.SettingsScreen? screenId, ref List<UISettingsGroup> __result)
        {
            try
            {
                if (screenId is not null && screenId == SettingsScreenId)
                {
                    Main.Logger.NativeLog($"Returning mod settings for screen {screenId}.");
                    __result = ModSettings;
                }
            }
            catch (Exception e)
            {
                Main.Logger.LogException(e);
            }
        }
    }
}