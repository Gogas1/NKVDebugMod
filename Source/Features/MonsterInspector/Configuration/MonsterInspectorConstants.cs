using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace NKVDebugMod.Features.MonsterInspector.Configuration {
    internal static class MonsterInspectorConstants {
        public const string ConfigSectionName = "Monster Inspector";

        public const string ToggleInspectorConfigName = "Toggle monster inspector key";
        public static KeyboardShortcut ToggleInspectorConfigDefault = new KeyboardShortcut(UnityEngine.KeyCode.I, UnityEngine.KeyCode.LeftControl);

        public const string NextMonsterConfigName = "Next monster key";
        public static KeyboardShortcut NextMonsterConfigDefault = new KeyboardShortcut(UnityEngine.KeyCode.RightArrow, UnityEngine.KeyCode.LeftAlt);

        public const string PrevMonsterConfigName = "Prev monster key";
        public static KeyboardShortcut PrevMonsterConfigDefault = new KeyboardShortcut(UnityEngine.KeyCode.LeftArrow, UnityEngine.KeyCode.LeftAlt);

        public const string SelectClosestMonsterConfigName = "Select closest monster key";
        public static KeyboardShortcut SelectClosestMonsterConfigDefault = new KeyboardShortcut(UnityEngine.KeyCode.UpArrow, UnityEngine.KeyCode.LeftAlt);
    }
}
