using RCGMaker.Runtime.LifeCycle;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NKVDebugMod.Features.MonsterInspector {
    internal static class MonsterInspectorFeature {
        private static GameObject _inspector = null!;
        public static void Initialize() {
            _inspector = new GameObject("MonsterInspector");
            _inspector.AddComponent<MonsterInspector>();
            RCGDontDestroyForever.DontDestroyOnLoad(_inspector);
            _inspector.hideFlags = HideFlags.HideAndDontSave;
        }

        public static void Destroy() {
            GameObject.Destroy(_inspector);
        }
    }
}
