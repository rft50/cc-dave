using System.Runtime.CompilerServices;
using Mono.Collections.Generic;

namespace Jester;

public static class CardPlayTracker
{

    public static IEnumerable<Card> GetCardPlaysThisTurn(State s, Combat c)
    {
        return Load(s, c);
    }

    public static void RegisterCardPlay(Card card, State s, Combat c)
    {
        var cards = Load(s, c).ToList();
        cards.Add(card);
        Save(cards, s);
    }

    public static void ClearCardPlays(State s)
    {
        Save(new List<Card>(), s);
    }
    
    private static IEnumerable<Card> Load(State s, Combat c)
    {
        var ids = ModManifest.KokoroApi.ObtainExtensionData<List<int>>(c, "CardTracking.Ids");
        var singleCards = ModManifest.KokoroApi.ObtainExtensionData<List<Card>>(c, "CardTracking.SingleCards");

        var allCards = c.hand
            .Union(s.deck)
            .Union(c.discard)
            .Union(c.exhausted)
            .Union(singleCards)
            .DistinctBy(ca => ca.uuid)
            .ToList();
        
        var cards = ids
            .Select(id => allCards.FirstOrDefault(ca => ca.uuid == id))
            .Where(ca => ca != null)
            .Cast<Card>();

        return cards;
    }

    private static void Save(IEnumerable<Card> cards, State state)
    {
        if (state?.route is not Combat combat) return;

        var enumerable = cards.ToList();
        var ids = enumerable
            .Select(c => c.uuid)
            .ToList();
        var singleUse = enumerable
            .Where(c => c.singleUseOverride == true || c.GetData(state).singleUse)
            .ToList();
        
        ModManifest.KokoroApi.SetExtensionData(combat, "CardTracking.Ids", ids);
        ModManifest.KokoroApi.SetExtensionData(combat, "CardTracking.SingleCards", singleUse);
    }
}