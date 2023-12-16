namespace Jester;

public static class CardPlayTracker
{
    private static bool _loaded = false;

    private static readonly List<Card> CardsPlayedThisTurn = new();

    public static IEnumerable<Card> GetCardPlaysThisTurn(State s, Combat c)
    {
        if (!_loaded)
            Load(s, c);
        return CardsPlayedThisTurn;
    }

    public static void RegisterCardPlay(Card card)
    {
        CardsPlayedThisTurn.Add(card);
        Save();
    }

    public static void ClearCardPlays()
    {
        CardsPlayedThisTurn.Clear();
        Save();
    }
    
    private static void Load(State s, Combat c)
    {
        _loaded = true;
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
            .Where(ca => ca != null);
        
        CardsPlayedThisTurn.InsertRange(0, cards!);
    }

    private static void Save()
    {
        var state = StateExt.Instance;
        if (state?.route is not Combat combat) return;
        
        if (!_loaded)
            Load(state, combat);

        var ids = CardsPlayedThisTurn
            .Select(c => c.uuid)
            .ToList();
        var singleUse = CardsPlayedThisTurn
            .Where(c => c.singleUseOverride == true || c.GetData(state).singleUse)
            .ToList();
        
        ModManifest.KokoroApi.SetExtensionData(combat, "CardTracking.Ids", ids);
        ModManifest.KokoroApi.SetExtensionData(combat, "CardTracking.SingleCards", singleUse);
    }
}