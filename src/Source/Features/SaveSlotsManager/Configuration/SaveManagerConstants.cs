using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NKVDebugMod.Features.SaveSlotsManager.Configuration {
    internal static class SaveManagerConstants {
        public const string CONFIG_SECTION_NAME = "Save Manager";

        public const string OPEN_MANAGER_CONFIG_NAME = "Open save manager";
        public static KeyboardShortcut OpenManagerConfigDefault = new KeyboardShortcut(KeyCode.N, KeyCode.LeftControl);

        public const string QUICK_SAVE_CONFIG_NAME = "Quick save";
        public static KeyboardShortcut QuickSaveConfigDefault = new KeyboardShortcut(KeyCode.F5, KeyCode.LeftControl);

        public const string QUICK_LOAD_CONFIG_NAME = "Quick load";
        public static KeyboardShortcut QuickLoadConfigDefault = new KeyboardShortcut(KeyCode.F9, KeyCode.LeftControl);

        public const string SAVE_CURRENT_POSITION_CONFIG_NAME = "Save on current position";
        public const bool SAVE_CURRENT_POSITION_CONFIG_DEFAULT = false;
    }
}
