using HarmonyLib;

namespace Jester.Patches;

[HarmonyPatch(typeof(Combat))]
public class CombatPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("TryPlayCard")]
    public static void TryPlayCard(Card card, bool __result)
    {
        if (__result)
            CardPlayTracker.RegisterCardPlay(card);
    }
}