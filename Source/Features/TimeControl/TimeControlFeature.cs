using RCGMaker.Runtime.LifeCycle;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityExplorer.Inspectors.MouseInspectors;

namespace NKVDebugMod.Features.TimeControl {
    internal static class TimeControlFeature {

        private static GameObject _timeController = null!;

        public static void Initialize() {
            _timeController = new GameObject("TimeController");
            _timeController.AddComponent<TimeController>();
            RCGDontDestroyForever.DontDestroyOnLoad(_timeController);
            _timeController.hideFlags = HideFlags.HideAndDontSave;
        }

        public static void Destroy() {
            GameObject.Destroy(_timeController);
        }
    }
}
