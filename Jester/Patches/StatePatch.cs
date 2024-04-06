using HarmonyLib;
using Jester.Cards;

namespace Jester.Patches;

[HarmonyPatch(typeof(State))]
public class StatePatch
{
    [HarmonyPostfix]
    [HarmonyPatch("PopulateRun")]
    public static void PopulateRun(State __instance)
    {
        var jesterDeck = (Deck?)ModManifest.JesterDeck?.Id;
        
        if (jesterDeck == null) return;

        if (__instance.characters.All(c => c.deckType != jesterDeck)) return;
        
        __instance.SendCardToDeck(new CommonOffensiveJoker());
        if (Random.Shared.NextDouble() <= 0.5)
            __instance.SendCardToDeck(new CommonDefensiveJoker());
        else
            __instance.SendCardToDeck(new CommonUtilityJoker());
    }
}