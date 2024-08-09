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
        ModManifest.Helper.ModData.SetModData(__instance, "Settings", ModManifest.Settings.ProfileBased.Current);
        
        var jesterDeck = (Deck?)ModManifest.JesterDeck?.Id;
        
        if (jesterDeck == null) return;

        if (__instance.characters.All(c => c.deckType != jesterDeck)) return;

        if (ModManifest.MoreDifficultiesApi?.AreAltStartersEnabled(__instance, jesterDeck.Value) == true)
        {
            __instance.SendCardToDeck(new CommonDefensiveJoker());
            __instance.SendCardToDeck(new CommonUtilityJoker());
        }
        else
        {
            __instance.SendCardToDeck(new CommonOffensiveJoker());
            if (__instance.rngCardOfferings.Next() <= 0.5)
                __instance.SendCardToDeck(new CommonDefensiveJoker());
            else
                __instance.SendCardToDeck(new CommonUtilityJoker());
        }
    }
}