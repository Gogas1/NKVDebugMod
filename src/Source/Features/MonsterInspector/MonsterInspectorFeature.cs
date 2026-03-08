using NKVDebugMod.Features.MonsterInspector.Configuration;
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
            var inspector = _inspector.AddComponent<MonsterInspector>();
            MonsterInspectorConfiguration.Init();
            inspector.Hook();
            RCGDontDestroyForever.DontDestroyOnLoad(_inspector);
            _inspector.hideFlags = HideFlags.HideAndDontSave;
        }

        public static void Destroy() {
            GameObject.Destroy(_inspector);
        }
    }
}
