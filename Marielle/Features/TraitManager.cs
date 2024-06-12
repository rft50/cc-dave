using System.Collections.Generic;
using FSPRO;
using HarmonyLib;
using Nickel;

namespace Marielle.Features;

[HarmonyPatch]
public class TraitManager
{
    internal static bool HandleFleeting = true;
    public TraitManager()
    {
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(AEndTurn), "Begin")]
    private static void AEndTurn_Begin_Postfix(G g, State s, Combat c) {
        if (HandleFleeting)
            c.QueueImmediate(new AExhaustFleeting());
    }
}

public class AExhaustFleeting : CardAction
{
    static readonly IModCards CardsHelper = ModEntry.Instance.Helper.Content.Cards;
    public override void Begin(G g, State s, Combat c)
    {
        timer = 0.0;
        List<Card> toExhaust = [];
        foreach (Card card in c.hand) {
            if (CardsHelper.IsCardTraitActive(s, card, ModEntry.Instance.Fleeting)) {
                toExhaust.Add(card);
            }
        }
        foreach (Card card in toExhaust) {
            card.ExhaustFX();
            Audio.Play(Event.CardHandling);
            c.hand.Remove(card);
            c.SendCardToExhaust(s, card);
            timer = 0.3;
        }	
    }
}