using Cysharp.Threading.Tasks;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NKVDebugMod.Features.SaveSlotsManager.HarmonyPatches {
    [HarmonyPatch]
    internal static class SaveManagerPatches {

        private static MethodBase TargetMethod() {
            // Source - https://stackoverflow.com/a/77701064
            // Posted by zonni, modified by community. See post 'Timeline' for change history
            // Retrieved 2026-03-07, License - CC BY-SA 4.0

            var targetMethod = typeof(SaveManager).GetMethod(nameof(SaveManager.LoadAllSaveSlotMetaForMainMenu), BindingFlags.Instance | BindingFlags.Public);
            var stateMachineAttr = targetMethod.GetCustomAttribute<AsyncStateMachineAttribute>();
            var moveNextMethod = stateMachineAttr.StateMachineType.GetMethod("MoveNext", BindingFlags.NonPublic | BindingFlags.Instance);

            return moveNextMethod;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            foreach (var instruction in instructions) {
                if(instruction.opcode == OpCodes.Ldc_I4_5) {
                    yield return new CodeInstruction(OpCodes.Ldc_I4_6);
                }
                else {
                    yield return instruction;

                }
            }
        }
    }
}
