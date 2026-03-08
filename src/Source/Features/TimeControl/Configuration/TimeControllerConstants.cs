using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace NKVDebugMod.Features.TimeControl.Configuration {
    internal static class TimeControllerConstants {
        public const string ConfigSectionName = "Time Control";

        public const string ToggleTimePanelConfigName = "Toggle time control panel";
        public static KeyboardShortcut ToggleInspectorConfigDefault = new KeyboardShortcut(UnityEngine.KeyCode.T, UnityEngine.KeyCode.LeftControl);

        public const string AdvanceFramesConfigName = "Advance frames";
        public static KeyboardShortcut AdvanceFramesConfigDefault = new KeyboardShortcut(UnityEngine.KeyCode.RightArrow, UnityEngine.KeyCode.LeftControl);

        public const int ADVANCE_FRAMES_DEFAULT = 1;

        public const string AdvanceTimeConfigName = "Advance time";
        public static KeyboardShortcut AdvanceTimeConfigDefault = new KeyboardShortcut(UnityEngine.KeyCode.H, UnityEngine.KeyCode.LeftControl);

        public const float ADVANCE_TIME_DEFAULT = 0.2f;

        public const string PauseResumeConfigName = "Pause/Resume";
        public static KeyboardShortcut PauseResumeConfigDefault = new KeyboardShortcut(UnityEngine.KeyCode.LeftControl, UnityEngine.KeyCode.LeftAlt);

        public const string ToggleTimeScaleConfigName = "Toggle time scaling";
        public static KeyboardShortcut ToggleTimeScaleConfigDefault = new KeyboardShortcut(UnityEngine.KeyCode.LeftAlt, UnityEngine.KeyCode.LeftControl);
    }
}
