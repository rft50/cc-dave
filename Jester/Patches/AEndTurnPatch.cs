using HarmonyLib;
using Jester.Cards;

namespace Jester.Patches;

[HarmonyPatch(typeof(AEndTurn))]
public class AEndTurnPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("Begin")]
    public static void BeginPrefix(out IEnumerable<Card>? __state)
    {
        __state = null;
        var state = StateExt.Instance;
        if (state?.route is not Combat combat) return;
        var hand = combat.hand;
        var isOnlyRetain = hand.All(c => c.retainOverride == true || c.GetData(state).retain);

        if (!isOnlyRetain) return;
        if (hand.Count == 1 && hand[0].GetType() == typeof(CurtainCall))
        {
            __state = hand;
        }
        else
        {
            __state = hand.Where(card => card.GetType() == typeof(CurtainCall) && card.upgrade == Upgrade.A);
        }
    }
    
    [HarmonyPostfix]
    [HarmonyPatch("Begin")]
    public static void BeginPostfix(IEnumerable<Card>? __state)
    {
        CardPlayTracker.ClearCardPlays();

        var state = StateExt.Instance;
        if (state?.route is not Combat combat) return;

        if (__state == null) return;
        foreach (var card in __state)
        {
            combat.Queue(new List<CardAction>
            {
                new AAttack
                {
                    damage = card.GetDmg(state, 1)
                }
            });
        }
    }
}