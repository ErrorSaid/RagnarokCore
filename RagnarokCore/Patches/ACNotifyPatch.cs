using HarmonyLib;
using Exiled.API.Features;

namespace RagnarokCore.Patches
{
    [HarmonyPatch(typeof(PlayerMovementSync), nameof(PlayerMovementSync.AntiCheatKillPlayer))]
    public static class ACNotifyPatch
    {
        public static void Prefix(PlayerMovementSync __instance, string message, string code) => Log.Warn($"AntiCheatKill Detectado: {Player.Get(__instance._hub)?.Nickname} [{message}({code})]");
    }
}