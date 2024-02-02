using System;
using System.Reflection;
using HarmonyLib;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.Journal;
using Kingmaker.UI.MVVM._PCView.Settings;
using Kingmaker.UI.MVVM._PCView.Settings.Entities;
using Kingmaker.UI.MVVM._PCView.Settings.Entities.Decorative;
using Kingmaker.UI.MVVM._VM.Settings;
using Kingmaker.UI.MVVM._VM.Settings.Entities;
using Kingmaker.UI.MVVM._VM.Settings.Entities.Decorative;
using Kingmaker.UI.MVVM._VM.Settings.Entities.Difficulty;
using Kingmaker.UI.SettingsUI;
using Kingmaker.Utility;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.MVVM;
using Owlcat.Runtime.UI.VirtualListSystem;
using Owlcat.Runtime.UI.VirtualListSystem.ElementSettings;
using SpeechMod.External.ModMenu.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SpeechMod.External.ModMenu.NewTypes;

internal class SettingsEntityPatches
{
    internal static readonly FieldInfo OverrideType =
        AccessTools.Field(typeof(VirtualListLayoutElementSettings), "m_OverrideType");

    /// <summary>
    /// Patch to return the correct view model for <see cref="UISettingsEntityImage"/>
    /// </summary>
    [HarmonyPatch(typeof(SettingsVM))]
    static class SettingsVM_Patch
    {
        [HarmonyPatch(nameof(SettingsVM.GetVMForSettingsItem)), HarmonyPrefix]
        static bool Prefix(
            UISettingsEntityBase uiSettingsEntity, ref VirtualListElementVMBase __result)
        {
            try
            {
                if (uiSettingsEntity is UISettingsEntityImage imageEntity)
                {
                    Main.Logger.NativeLog("Returning SettingsEntityImageVM.");
                    __result = new SettingsEntityImageVM(imageEntity);
                    return false;
                }
                if (uiSettingsEntity is UISettingsEntityButton buttonEntity)
                {
                    Main.Logger.NativeLog("Returning SettingsEntityButtonVM.");
                    __result = new SettingsEntityButtonVM(buttonEntity);
                    return false;
                }
                if (uiSettingsEntity is UISettingsEntitySubHeader subHeaderEntity)
                {
                    Main.Logger.NativeLog("Returning SettingsEntitySubHeaderVM.");
                    __result = new SettingsEntitySubHeaderVM(subHeaderEntity);
                    return false;
                }
                if (uiSettingsEntity is UISettingsEntityDropdownButton dropdownButton)
                {
                    Main.Logger.NativeLog("Returning SettingsEntityDropdownButtonVM.");
                    __result = new SettingsEntityDropdownButtonVM(dropdownButton);
                    return false;
                }
            }
            catch (Exception e)
            {
                Main.Logger.LogException("SettingsVM.GetVMForSettingsItem", e);
            }
            return true;
        }

        [HarmonyPatch(nameof(SettingsVM.SwitchSettingsScreen)), HarmonyPostfix]
        static void Postfix(UISettingsManager.SettingsScreen settingsScreen, SettingsVM __instance)
        {
            try
            {
                if (settingsScreen != ModsMenuEntity.SettingsScreenId) { return; }
                Main.Logger.NativeLog("Configuring header buttons.");

                // Add all settings in each group to the corresponding expand/collapse button
                SettingsEntityCollapsibleHeaderVM headerVM = null;
                SettingsEntitySubHeaderVM subHeaderVM = null;
                for (int i = 0; i < __instance.m_SettingEntities.Count; i++)
                {
                    var entity = __instance.m_SettingEntities[i];
                    if (entity is SettingsEntitySubHeaderVM subHeader)
                    {
                        subHeaderVM = subHeader;
                        if (headerVM is not null)
                            headerVM.SettingsInGroup.Add(subHeaderVM); // Sub headers are nested in headers
                        continue;
                    }
                    else if (entity is SettingsEntityHeaderVM header)
                    {
                        headerVM = new SettingsEntityCollapsibleHeaderVM(header.Tittle);
                        __instance.m_SettingEntities[i] = headerVM;
                        subHeaderVM = null; // Make sure we stop counting sub header entries
                        continue;
                    }

                    if (headerVM is not null)
                        headerVM.SettingsInGroup.Add(entity);
                    if (subHeaderVM is not null)
                        subHeaderVM.SettingsInGroup.Add(entity);
                }
            }
            catch (Exception e)
            {
                Main.Logger.LogException("SettingsVM.SwitchSettingsScreen", e);
            }
        }
    }

    /// <summary>
    /// Patch to add new setting type prefabs.
    /// </summary>
    [HarmonyPatch(typeof(SettingsPCView.SettingsViews))]
    static class SettingsViews_Patch
    {
        [HarmonyPatch(nameof(SettingsPCView.SettingsViews.InitializeVirtualList)), HarmonyPrefix]
        static bool Prefix(SettingsPCView.SettingsViews __instance, VirtualListComponent virtualListComponent)
        {
            try
            {
                Main.Logger.NativeLog("Adding new type prefabs.");

                // Copy the bool settings
                var copyFrom = __instance.m_SettingsEntityBoolViewPrefab.gameObject;
                var imageTemplate = CreateImageTemplate(Object.Instantiate(copyFrom));
                var buttonTemplate =
                    CreateButtonTemplate(Object.Instantiate(copyFrom),
                        __instance.m_SettingsEntitySliderVisualPerceptionViewPrefab?.m_ResetButton);

                var headerTemplate =
                    CreateCollapsibleHeaderTemplate(
                        Object.Instantiate(__instance.m_SettingsEntityHeaderViewPrefab.gameObject));
                var subHeaderTemplate = CreateSubHeaderTemplate(Object.Instantiate(headerTemplate.gameObject));

                // Copy dropdown since you know, it seems like close to dropdown button right?
                var dropdownButtonTemplate =
                    CreateDropdownButtonTemplate(
                        Object.Instantiate(__instance.m_SettingsEntityDropdownViewPrefab.gameObject),
                        __instance.m_SettingsEntitySliderVisualPerceptionViewPrefab?.m_ResetButton);

                virtualListComponent.Initialize(new IVirtualListElementTemplate[]
                {
                    new VirtualListElementTemplate<SettingsEntityHeaderVM>(__instance.m_SettingsEntityHeaderViewPrefab),
                    new VirtualListElementTemplate<SettingsEntityBoolVM>(__instance.m_SettingsEntityBoolViewPrefab),
                    new VirtualListElementTemplate<SettingsEntityDropdownVM>(__instance.m_SettingsEntityDropdownViewPrefab, 0),
                    new VirtualListElementTemplate<SettingsEntitySliderVM>(__instance.m_SettingsEntitySliderViewPrefab, 0),
                    new VirtualListElementTemplate<SettingEntityKeyBindingVM>(__instance.m_SettingEntityKeyBindingViewPrefab),
                    new VirtualListElementTemplate<SettingsEntityDropdownVM>(__instance.m_SettingsEntityDropdownDisplayModeViewPrefab, 1),
                    new VirtualListElementTemplate<SettingsEntityDropdownGameDifficultyVM>(__instance.m_SettingsEntityDropdownGameDifficultyViewPrefab, 0),
                    new VirtualListElementTemplate<SettingsEntitySliderVM>(__instance.m_SettingsEntitySliderVisualPerceptionViewPrefab, 1),
                    new VirtualListElementTemplate<SettingsEntitySliderVM>(__instance.m_SettingsEntitySliderVisualPerceptionWithImagesViewPrefab, 2),
                    new VirtualListElementTemplate<SettingsEntityStatisticsOptOutVM>(__instance.m_SettingsEntityStatisticsOptOutViewPrefab),
                    new VirtualListElementTemplate<SettingsEntityImageVM>(imageTemplate),
                    new VirtualListElementTemplate<SettingsEntityButtonVM>(buttonTemplate),
                    new VirtualListElementTemplate<SettingsEntityCollapsibleHeaderVM>(headerTemplate),
                    new VirtualListElementTemplate<SettingsEntitySubHeaderVM>(subHeaderTemplate),
                    new VirtualListElementTemplate<SettingsEntityDropdownButtonVM>(dropdownButtonTemplate, 0),
                });
            }
            catch (Exception e)
            {
                Main.Logger.LogException("SettingsViews_Patch", e);
            }
            return false;
        }

        private static SettingsEntityButtonView CreateButtonTemplate(GameObject prefab, OwlcatButton buttonPrefab)
        {
            Main.Logger.NativeLog("Creating button template.");

            // Destroy the stuff we don't want from the source prefab
            Object.DestroyImmediate(prefab.GetComponent<SettingsEntityBoolPCView>());
            Object.DestroyImmediate(prefab.transform.Find("MultiButton").gameObject);
            Object.DontDestroyOnLoad(prefab);

            OwlcatButton buttonControl = null;
            TextMeshProUGUI buttonLabel = null;

            // Add in our own button
            if (buttonPrefab != null)
            {
                var button = Object.Instantiate(buttonPrefab.gameObject, prefab.transform);
                buttonControl = button.GetComponent<OwlcatButton>();
                buttonLabel = button.GetComponentInChildren<TextMeshProUGUI>();

                var layout = button.AddComponent<LayoutElement>();
                layout.ignoreLayout = true;

                var rect = button.transform as RectTransform;

                rect.anchorMin = new(1, 0.5f);
                rect.anchorMax = new(1, 0.5f);
                rect.pivot = new(1, 0.5f);

                rect.anchoredPosition = new(-55, 0);
                rect.sizeDelta = new(430, 45);
            }

            // Add our own View (after destroying the Bool one)
            var templatePrefab = prefab.AddComponent<SettingsEntityButtonView>();

            // Wire up the fields that would have been deserialized if coming from a bundle
            templatePrefab.HighlightedImage =
                prefab.transform.Find("HighlightedImage").gameObject.GetComponent<Image>();
            templatePrefab.Title =
                prefab.transform.Find("HorizontalLayoutGroup/Text").gameObject.GetComponent<TextMeshProUGUI>();
            templatePrefab.Button = buttonControl;
            templatePrefab.ButtonLabel = buttonLabel;

            return templatePrefab;
        }

        private static SettingsEntityImageView CreateImageTemplate(GameObject prefab)
        {
            Main.Logger.NativeLog("Creating image template.");

            //Destroy the stuff we don't want from the source prefab
            Object.DestroyImmediate(prefab.GetComponent<SettingsEntityBoolPCView>());
            Object.DestroyImmediate(prefab.transform.Find("MultiButton").gameObject);
            Object.DestroyImmediate(prefab.transform.Find("HorizontalLayoutGroup").gameObject);
            Object.DestroyImmediate(prefab.transform.Find("HighlightedImage").gameObject);
            Object.DontDestroyOnLoad(prefab);

            // Add our own View (after destroying the Bool one)
            var templatePrefab = prefab.AddComponent<SettingsEntityImageView>();

            // Create an imagePrefab as a child of the view so it can be scaled independently
            var imagePrefab = new GameObject("banner", typeof(RectTransform));
            imagePrefab.transform.SetParent(templatePrefab.transform, false);

            // Wire up the fields that would have been deserialized if coming from a bundle
            templatePrefab.Icon = imagePrefab.AddComponent<Image>();
            templatePrefab.Icon.preserveAspect = true;
            templatePrefab.TopBorder = prefab.transform.Find("TopBorderImage").gameObject;

            return templatePrefab;
        }

        private static SettingsEntityCollapsibleHeaderView CreateCollapsibleHeaderTemplate(GameObject prefab)
        {
            Main.Logger.NativeLog("Creating collapsible header template.");

            // Destroy the stuff we don't want from the source prefab
            Object.DestroyImmediate(prefab.GetComponent<SettingsEntityHeaderView>());
            Object.DontDestroyOnLoad(prefab);

            var buttonPC = prefab.GetComponentInChildren<ExpandableCollapseMultiButtonPC>();
            var buttonPrefab = buttonPC.gameObject;
            buttonPrefab.transform.Find("_CollapseArrowImage").gameObject.SetActive(true);
            var button = buttonPrefab.GetComponent<OwlcatMultiButton>();
            button.Interactable = true;

            // Add our own View
            var templatePrefab = prefab.AddComponent<SettingsEntityCollapsibleHeaderView>();
            templatePrefab.Title = prefab.transform.FindRecursive("Label").GetComponent<TextMeshProUGUI>();
            templatePrefab.Button = button;
            templatePrefab.ButtonPC = buttonPC;
            return templatePrefab;
        }

        // Prefab from the SettingsEntityCollapsibleHeaderView
        private static SettingsEntitySubHeaderView CreateSubHeaderTemplate(GameObject prefab)
        {
            Main.Logger.NativeLog("Creating sub header template.");

            // Destroy the stuff we don't want from the source prefab
            Object.DestroyImmediate(prefab.GetComponent<SettingsEntityCollapsibleHeaderView>());
            Object.DontDestroyOnLoad(prefab);

            // Add our own view
            var templatePrefab = prefab.AddComponent<SettingsEntitySubHeaderView>();
            templatePrefab.Title = prefab.transform.FindRecursive("Label").GetComponent<TextMeshProUGUI>();
            templatePrefab.Button = prefab.GetComponentInChildren<OwlcatMultiButton>();
            templatePrefab.ButtonPC = prefab.GetComponentInChildren<ExpandableCollapseMultiButtonPC>();
            return templatePrefab;
        }

        private static SettingsEntityDropdownButtonView CreateDropdownButtonTemplate(
            GameObject prefab, OwlcatButton buttonPrefab)
        {
            Main.Logger.NativeLog("Creating dropdown button template.");

            // Destroy the stuff we don't want from the source prefab
            Object.DestroyImmediate(prefab.GetComponent<SettingsEntityDropdownPCView>());
            Object.DestroyImmediate(prefab.transform.Find("SetConnectionMarkerIamSet").gameObject);
            Object.DontDestroyOnLoad(prefab);

            OwlcatButton buttonControl = null;
            TextMeshProUGUI buttonLabel = null;

            // Add in our own button
            if (buttonPrefab != null)
            {
                var button = Object.Instantiate(buttonPrefab.gameObject, prefab.transform);
                buttonControl = button.GetComponent<OwlcatButton>();
                buttonLabel = button.GetComponentInChildren<TextMeshProUGUI>();

                var layout = button.AddComponent<LayoutElement>();
                layout.ignoreLayout = true;

                var rect = button.transform as RectTransform;

                rect.anchorMin = new(1, 0.5f);
                rect.anchorMax = new(1, 0.5f);
                rect.pivot = new(1, 0.5f);

                rect.anchoredPosition = new(-510, 0);
                rect.sizeDelta = new(215, 45);
            }

            // Add our own View (after destroying the Bool one)
            var templatePrefab = prefab.AddComponent<SettingsEntityDropdownButtonView>();

            // Wire up the fields that would have been deserialized if coming from a bundle
            templatePrefab.HighlightedImage =
                prefab.transform.Find("HighlightedImage").gameObject.GetComponent<Image>();
            templatePrefab.Title =
                prefab.transform.Find("HorizontalLayoutGroup/Text").gameObject.GetComponent<TextMeshProUGUI>();
            templatePrefab.Dropdown = prefab.GetComponentInChildren<TMP_Dropdown>();
            templatePrefab.Button = buttonControl;
            templatePrefab.ButtonLabel = buttonLabel;

            return templatePrefab;
        }
    }
}