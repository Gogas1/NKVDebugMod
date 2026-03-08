using BepInEx.Logging;
using System;

namespace NKVDebugMod;

internal static class Log {
    private static ManualLogSource? logSource;

    internal static void Init(ManualLogSource logSource) {
        Log.logSource = logSource;
    }

    internal static void Debug(object data) => logSource?.LogDebug(data);

    internal static void Error(object data) => logSource?.LogError(data);

    internal static void Fatal(object data) => logSource?.LogFatal(data);

    internal static void Info(object data) => logSource?.LogInfo(data);

    internal static void Message(object data) => logSource?.LogMessage(data);

    internal static void Warning(object data) => logSource?.LogWarning(data);
    internal static void Exception(Exception ex, string message = "") {
        if (string.IsNullOrEmpty(message)) {
            logSource?.LogError($"{ex.Message}, {ex.StackTrace}");
        } else {
            logSource?.LogError($"{message}. {ex.Message}, {ex.StackTrace}");
        }
    }
}