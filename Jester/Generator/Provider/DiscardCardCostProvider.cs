using Jester.Api;

namespace Jester.Generator.Provider;

public class DiscardCardCostProvider : IProvider
{
    public IList<IEntry> GetEntries(IJesterRequest request)
    {
        if (!request.Whitelist.Contains("cost")) return new List<IEntry>();

        var minCost = request.MinCost;
        var maxCost = request.MaxCost;
        
        return new List<int> { 1, 2, 3, 4, 5 }
            .Select(i => new DiscardCardCostEntry(i))
            .Where(e => ModManifest.JesterApi.GetJesterUtil().InRange(minCost, e.GetCost(), maxCost))
            .ToList<IEntry>();
    }
    
    public class DiscardCardCostEntry : IEntry
    {
        public int Amount { get; }

        public DiscardCardCostEntry(int amount)
        {
            Amount = amount;
        }

        public ISet<string> Tags => new HashSet<string>
        {
            "cost"
        };

        public int GetActionCount() => 1;

        public IList<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new ADiscard
            {
                count = Amount,
                ignoreRetain = true
            }
        };

        public int GetCost() => Amount * -10;

        public IEntry? GetUpgradeA(IJesterRequest request, out int cost)
        {
            if (Amount <= 1)
            {
                cost = 0;
                return null;
            }

            cost = 10;
            return new DiscardCardCostEntry(Amount - 1);
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