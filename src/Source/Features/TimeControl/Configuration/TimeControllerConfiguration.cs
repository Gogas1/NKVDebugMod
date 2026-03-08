using BepInEx.Configuration;
using NineSolsAPI;
using System;
using System.Collections.Generic;
using System.Text;

using static NKVDebugMod.NKVDebugMod;
using static NKVDebugMod.Features.TimeControl.Configuration.TimeControllerConstants;

namespace NKVDebugMod.Features.TimeControl.Configuration {
    internal static class TimeControllerConfiguration {
        private static ConfigEntry<KeyboardShortcut>? _toggleTimeControlPanel;
        private static ConfigEntry<KeyboardShortcut>? _toggleTimeScaleKey;
        private static ConfigEntry<KeyboardShortcut>? _advanceFramesKey;
        private static ConfigEntry<KeyboardShortcut>? _advanceTimeKey;
        private static ConfigEntry<KeyboardShortcut>? _pauseResumeKey;

        public static event Action? OnToggleTimeControlInvoked;
        public static event Action? OnToggleTimeScaleInvoked;
        public static event Action? OnAdvanceFramesInvoked;
        public static event Action? OnAdvanceTimeInvoked;
        public static event Action? OnResumePausedInvoked;

        internal static void Init() {
            _toggleTimeControlPanel = ModConfig.Bind(ConfigSectionName, ToggleTimePanelConfigName, ToggleInspectorConfigDefault);
            _advanceFramesKey = ModConfig.Bind(ConfigSectionName, AdvanceFramesConfigName, AdvanceFramesConfigDefault);
            _advanceTimeKey = ModConfig.Bind(ConfigSectionName, AdvanceTimeConfigName, AdvanceTimeConfigDefault);
            _pauseResumeKey = ModConfig.Bind(ConfigSectionName, PauseResumeConfigName, PauseResumeConfigDefault);
            _toggleTimeScaleKey = ModConfig.Bind(ConfigSectionName, ToggleTimeScaleConfigName, ToggleTimeScaleConfigDefault);

            if (TimeController.Instance != null) {
                KeybindManager.Add(TimeController.Instance, () => OnToggleTimeScaleInvoked?.Invoke(), () => _toggleTimeScaleKey.Value);
                KeybindManager.Add(TimeController.Instance, () => OnToggleTimeControlInvoked?.Invoke(), () => _toggleTimeControlPanel.Value);
                KeybindManager.Add(TimeController.Instance, () => OnAdvanceFramesInvoked?.Invoke(), () => _advanceFramesKey.Value);
                KeybindManager.Add(TimeController.Instance, () => OnAdvanceTimeInvoked?.Invoke(), () => _advanceTimeKey.Value);
                KeybindManager.Add(TimeController.Instance, () => OnResumePausedInvoked?.Invoke(), () => _pauseResumeKey.Value);
            }
        }
    }
}
