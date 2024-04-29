using Jester.Api;

namespace Jester.Generator.Provider.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class DiscardCardCostProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        if (!request.Whitelist.Contains("cost")) return new List<(double, IEntry)>();
        var costBase = 8 - request.CardData.cost;

        return Enumerable.Range(1, 5)
            .Select(i => (0.2, new DiscardCardCostEntry
            {
                CostBase = costBase,
                Amount = i
            } as IEntry));
    }
    
    private class DiscardCardCostEntry : IEntry
    {
        public int CostBase { get; init; }

        public int Amount { get; init; }

        public IReadOnlySet<string> Tags =>
            new HashSet<string>
            {
                "cost",
                "discardCost"
            };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new ADiscard
            {
                count = Amount,
                ignoreRetain = true
            }
        };

        public int GetCost() => -Amount * CostBase;

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            if (Amount <= 1) return new List<(double, IEntry)>();
            return new List<(double, IEntry)>
            {
                (1, new DiscardCardCostEntry
                {
                    CostBase = CostBase,
                    Amount = Amount - 1
                })
            };
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("discardCost");
        }
    }
}