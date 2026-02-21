using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using HarmonyLib;
using NineSolsAPI;
using NineSolsAPI.Utils;
using NKVDebugMod.Features.MonsterInspector;
using NKVDebugMod.Features.TimeControl;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NKVDebugMod;

[BepInDependency(NineSolsAPICore.PluginGUID)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class NKVDebugMod : BaseUnityPlugin {

    private Harmony harmony = null!;

    internal static bool IsUnityExplorerPresent;
    internal static ConfigFile ModConfig = null!;

    private void Awake() {
        ModConfig = Config;
        Log.Init(Logger);
        RCGLifeCycle.DontDestroyForever(gameObject);
        gameObject.hideFlags = HideFlags.HideAndDontSave;

        harmony = Harmony.CreateAndPatchAll(typeof(NKVDebugMod).Assembly);

        MonsterInspectorFeature.Initialize();
        TimeControlFeature.Initialize();

        if(Chainloader.PluginInfos.ContainsKey("com.sinai.unityexplorer")) {
            IsUnityExplorerPresent = true;
        }

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void OnDestroy() {
        harmony.UnpatchSelf();
        MonsterInspectorFeature.Destroy();
        TimeControlFeature.Destroy();
    }
}