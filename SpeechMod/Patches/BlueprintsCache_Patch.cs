using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;
using System;

namespace SpeechMod.Patches;

/// <summary>
/// Adds settings through ModMenu.
/// </summary>
[HarmonyPatch(typeof(BlueprintsCache))]
static class BlueprintsCache_Patch
{
    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(nameof(BlueprintsCache.Init)), HarmonyPostfix]
    static void Postfix()
    {
        try
        {

        }
        catch (Exception e)
        {
            Main.Logger.LogException("BlueprintsCache.Init", e);
        }
    }
}

