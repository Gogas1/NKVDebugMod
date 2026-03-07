using NKVDebugMod.Features.SaveSlotsManager.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NKVDebugMod.Features.SaveSlotsManager {
    internal static class SaveManagerFeature {
        private static GameObject _saveManagerObject = null!;

        public static void Initialize() {
            _saveManagerObject = new GameObject("NKVDM SaveManager");
            var saveManager = _saveManagerObject.AddComponent<SaveSlotsManager>();
            SaveManagerConfiguration.Init();
            saveManager.Hook();
            RCGLifeCycle.DontDestroyForever(_saveManagerObject);
            _saveManagerObject.hideFlags = HideFlags.HideAndDontSave;
        }

        public static void Destroy() {
            GameObject.Destroy(_saveManagerObject);
        }
    }
}
