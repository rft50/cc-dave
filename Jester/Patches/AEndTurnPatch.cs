using HarmonyLib;

namespace Jester.Patches;

[HarmonyPatch(typeof(AEndTurn))]
public class AEndTurnPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("Begin")]
    public static void Begin(State s)
    {
        CardPlayTracker.ClearCardPlays(s);
    }
}