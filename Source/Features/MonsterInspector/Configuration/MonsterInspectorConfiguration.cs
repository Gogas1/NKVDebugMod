using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static NKVDebugMod.NKVDebugMod;
using static NKVDebugMod.Features.MonsterInspector.Configuration.MonsterInspectorConstants;
using NineSolsAPI;

namespace NKVDebugMod.Features.MonsterInspector.Configuration {
    internal static class MonsterInspectorConfiguration {
        private static ConfigEntry<KeyboardShortcut>? _toggleMonsterInspector;
        private static ConfigEntry<KeyboardShortcut>? _nextMonsterKey;
        private static ConfigEntry<KeyboardShortcut>? _previousMonsterKey;
        private static ConfigEntry<KeyboardShortcut>? _selectClosestMonsterKey;

        public static Action? OnToggleMonsterInspectorInvoked;
        public static Action? OnNextMonsterInvoked;
        public static Action? OnPrevMonsterInvoked;
        public static Action? OnSelectClosestMonsterInvoked;

        internal static void Init() {
            _toggleMonsterInspector = ModConfig.Bind(ConfigSectionName, ToggleInspectorConfigName, ToggleInspectorConfigDefault);
            _nextMonsterKey = ModConfig.Bind(ConfigSectionName, NextMonsterConfigName, NextMonsterConfigDefault);
            _previousMonsterKey = ModConfig.Bind(ConfigSectionName, PrevMonsterConfigName, PrevMonsterConfigDefault);
            _selectClosestMonsterKey = ModConfig.Bind(ConfigSectionName, SelectClosestMonsterConfigName, SelectClosestMonsterConfigDefault);

            if (MonsterInspector.Instance != null) {
                KeybindManager.Add(MonsterInspector.Instance, () => OnToggleMonsterInspectorInvoked?.Invoke(), () => _toggleMonsterInspector.Value);
                KeybindManager.Add(MonsterInspector.Instance, () => OnNextMonsterInvoked?.Invoke(), () => _nextMonsterKey.Value);
                KeybindManager.Add(MonsterInspector.Instance, () => OnPrevMonsterInvoked?.Invoke(), () => _previousMonsterKey.Value);
                KeybindManager.Add(MonsterInspector.Instance, () => OnSelectClosestMonsterInvoked?.Invoke(), () => _selectClosestMonsterKey.Value);
            }
        }
    }
}
