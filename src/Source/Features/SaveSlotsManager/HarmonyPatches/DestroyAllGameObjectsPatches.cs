using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace NKVDebugMod.Features.SaveSlotsManager.HarmonyPatches {

    [HarmonyPatch(typeof(DestroyAllGameObjects))]
    internal static class DestroyAllGameObjectsPatches {

        [HarmonyPatch(nameof(DestroyAllGameObjects.BackToTitle))]
        [HarmonyPrefix]
        private static bool BackToTitle_Prefix() {
            if (!SaveSlotsManager.IsSaveSlotLoadRequested) {
                return true;
            }

            SaveManager.Instance.LoadWithCommand("saveslot5");
            SaveSlotsManager.IsSaveSlotLoadRequested = false;
            return false;
        }
    }
}
