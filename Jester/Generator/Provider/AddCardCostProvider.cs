namespace Jester.Generator.Provider;

public class AddCardCostProvider : IProvider
{
    public List<IEntry> GetEntries(JesterRequest request)
    {
        if (!request.Whitelist.Contains("cost")) return new List<IEntry>();

        var minCost = request.MinCost;
        var maxCost = request.MaxCost;
        
        var entries = new List<IEntry>();
        
        for (var i = 1; i <= 3; i++)
        {
            entries.Add(new AddCardCostEntry(new TrashFumes(), i, -15));
            entries.Add(new AddCardCostEntry(new ColorlessTrash(), i, -35));
        }

        return entries.Where(e => Util.InRange(minCost, e.GetCost(), maxCost))
            .ToList();
    }

    public class AddCardCostEntry : IEntry
    {
        public Card Card;
        public int Amount;
        public int Cost;

        public AddCardCostEntry(Card card, int amount, int cost)
        {
            Card = card;
            Amount = amount;
            Cost = cost;
        }

        public HashSet<string> Tags => new()
        {
            "cost",
            "addCard"
        };

        public int GetActionCount() => 1;

        public List<CardAction> GetActions(State s, Combat c) => new()
        {
            new AAddCard
            {
                card = Card.CopyWithNewId(),
                amount = Amount,
                destination = CardDestination.Deck,
                insertRandomly = true
            }
        };

        public int GetCost() => Amount * Cost;

        public IEntry? GetUpgradeA(JesterRequest request, out int cost)
        {
            if (Amount <= 1)
            {
                cost = 0;
                return null;
            }

            cost = -Cost;
            return new AddCardCostEntry(Card, Amount - 1, Cost);
        }

        public IEntry? GetUpgradeB(JesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public void AfterSelection(JesterRequest request)
        {
        }
    }
}