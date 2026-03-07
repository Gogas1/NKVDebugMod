using BepInEx.Configuration;
using NineSolsAPI;
using System;
using System.Collections.Generic;
using System.Text;
using static NKVDebugMod.Features.SaveSlotsManager.Configuration.SaveManagerConstants;

namespace NKVDebugMod.Features.SaveSlotsManager.Configuration {
    internal static class SaveManagerConfiguration {
        private static ConfigEntry<KeyboardShortcut>? _quickLoad;
        private static ConfigEntry<KeyboardShortcut>? _quickSave;
        private static ConfigEntry<KeyboardShortcut>? _openManager;

        private static ConfigEntry<bool>? _saveOnPosition;

        public static event Action? OnOpenManagerInvoked;
        public static event Action? OnQuickloadInvoked;
        public static event Action? OnQuickSaveInvoked;

        public static bool SaveOnPosition => _saveOnPosition != null ? _saveOnPosition.Value : false;

        internal static void Init() {
            _openManager = NKVDebugMod.ModConfig.Bind(CONFIG_SECTION_NAME, OPEN_MANAGER_CONFIG_NAME, OpenManagerConfigDefault);
            _quickLoad = NKVDebugMod.ModConfig.Bind(CONFIG_SECTION_NAME, QUICK_LOAD_CONFIG_NAME, QuickLoadConfigDefault);
            _quickSave = NKVDebugMod.ModConfig.Bind(CONFIG_SECTION_NAME, QUICK_SAVE_CONFIG_NAME, QuickSaveConfigDefault);
            _saveOnPosition = NKVDebugMod.ModConfig.Bind(CONFIG_SECTION_NAME, SAVE_CURRENT_POSITION_CONFIG_NAME, SAVE_CURRENT_POSITION_CONFIG_DEFAULT);

            if (SaveSlotsManager.Instance != null) {
                KeybindManager.Add(SaveSlotsManager.Instance, () => OnOpenManagerInvoked?.Invoke(), () => _openManager.Value);
                KeybindManager.Add(SaveSlotsManager.Instance, () => OnQuickloadInvoked?.Invoke(), () => _quickLoad.Value);
                KeybindManager.Add(SaveSlotsManager.Instance, () => OnQuickSaveInvoked?.Invoke(), () => _quickSave.Value);
            }
        }
    }
}
