using Jester.Api;

namespace Jester.Generator.Provider;

public class AddCardCostProvider : IProvider
{
    public IList<IEntry> GetEntries(IJesterRequest request)
    {
        if (!request.Whitelist.Contains("cost")) return new List<IEntry>();

        var minCost = request.MinCost;
        var maxCost = request.MaxCost;
        
        var entries = new List<IEntry>();
        var limit = request.CardData.cost >= 4 ? 2 : 1;
        
        for (var i = 1; i <= limit; i++)
        {
            entries.Add(new AddCardCostEntry(new TrashFumes(), i, -10));
            entries.Add(new AddCardCostEntry(new ColorlessTrash(), i, -20));
        }

        return entries.Where(e => ModManifest.JesterApi.GetJesterUtil().InRange(minCost, e.GetCost(), maxCost))
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

        public ISet<string> Tags => new HashSet<string>
        {
            "cost",
            "addCard"
        };

        public int GetActionCount() => 1;

        public IList<CardAction> GetActions(State s, Combat c) => new List<CardAction>
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

        public IEntry? GetUpgradeA(IJesterRequest request, out int cost)
        {
            if (Amount <= 1)
            {
                cost = 0;
                return null;
            }

            cost = -Cost;
            return new AddCardCostEntry(Card, Amount - 1, Cost);
        }

        public IEntry? GetUpgradeB(IJesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public void AfterSelection(IJesterRequest request)
        {
        }
    }
}