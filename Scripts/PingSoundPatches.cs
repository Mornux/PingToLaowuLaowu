using HarmonyLib;
using MegaCrit.Sts2.Core.Multiplayer.Game;

namespace PingToLaowuLaowu.Scripts;

// Original messages and their send debounce are left intact.
[HarmonyPatch(typeof(FlavorSynchronizer), nameof(FlavorSynchronizer.SendEndTurnPing))]
internal static class SendPingSoundPatch
{
    [HarmonyPostfix]
    private static void Postfix() => LaowuSoundPlayer.PlayRandom();
}

[HarmonyPatch(typeof(FlavorSynchronizer), "HandleEndTurnPingMessage")]
internal static class ReceivePingSoundPatch
{
    [HarmonyPostfix]
    private static void Postfix(ulong senderId, ulong ____localPlayerId)
    {
        // Avoid an extra playback if a transport ever echoes our own ping.
        if (senderId != ____localPlayerId)
            LaowuSoundPlayer.PlayRandom();
    }
}
