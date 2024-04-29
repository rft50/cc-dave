using Jester.Api;

namespace Jester.Generator.Provider.Drake;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class HeatCostProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        if (!request.Whitelist.Contains("cost")) return new List<(double, IEntry)>();
        if (!ModManifest.JesterApi.HasCharacterFlag("heat")) return new List<(double, IEntry)>();

        return Enumerable.Range(1, 3)
            .Select(i => (1.3, new HeatCostEntry
            {
                Count = i
            } as IEntry));
    }

    private class HeatCostEntry : IEntry
    {
        public int Count { get; set; }
        
        public IReadOnlySet<string> Tags { get; } = new HashSet<string>
        {
            "status",
            "heat",
            "cost"
        };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AStatus
            {
                status = Enum.Parse<Status>("heat"),
                statusAmount = Count,
                targetPlayer = true
            }
        };

        public int GetCost()
        {
            return Count * -7;
        }

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            if (Count <= 1) return new List<(double, IEntry)>();
            return new List<(double, IEntry)>
            {
                (1, new HeatCostEntry
                {
                    Count = Count - 1
                })
            };
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("heat");
        }
    }
}