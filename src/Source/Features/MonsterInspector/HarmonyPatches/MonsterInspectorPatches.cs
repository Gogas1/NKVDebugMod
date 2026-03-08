using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace NKVDebugMod.Features.MonsterInspector.HarmonyPatches {
    [HarmonyPatch(typeof(MonsterManager))]
    internal static class MonsterInspectorPatches {
        public static event Action<MonsterManager, MonsterBase>? OnRegisterPostifx;
        public static event Action<MonsterManager, MonsterBase>? OnUnregisterPostifx;


        [HarmonyPatch(nameof(MonsterManager.Register))]
        [HarmonyPostfix]
        private static void Register_Postfix(MonsterManager __instance, MonsterBase monster) {
            try {
                OnRegisterPostifx?.Invoke(__instance, monster);
            } catch (Exception e) {
                Log.Exception(e);
            }
        }

        [HarmonyPatch(nameof(MonsterManager.Unregister))]
        [HarmonyPostfix]
        private static void Unregister_Postfix(MonsterManager __instance, MonsterBase monster) {
            try {
                OnUnregisterPostifx?.Invoke(__instance, monster);
            } catch (Exception e) {
                Log.Exception(e);
            }
        }
    }
}
