using Jester.Api;

namespace Jester.Generator.Provider.Drake;

public class HeatCostProvider : IProvider
{
    public IList<IEntry> GetEntries(IJesterRequest request)
    {
        if (!request.Whitelist.Contains("cost")) return new List<IEntry>();
        if (!ModManifest.JesterApi.HasCharacterFlag("heat")) return new List<IEntry>();
        
        var minCost = request.MinCost;
        var maxCost = request.MaxCost;

        return new List<IEntry>
            {
                new HeatCostEntry(1),
                new HeatCostEntry(2),
                new HeatCostEntry(3)
            }.Where(e => ModManifest.JesterApi.GetJesterUtil().InRange(minCost, e.GetCost(), maxCost))
            .ToList();
    }

    public class HeatCostEntry : IEntry
    {
        public int Count { get; set; }

        public HeatCostEntry()
        {
            
        }

        public HeatCostEntry(int count)
        {
            Count = count;
        }
        public ISet<string> Tags { get; } = new HashSet<string>
        {
            "status",
            "heat",
            "cost",
            "weighted"
        };

        public int GetActionCount() => 1;

        public IList<CardAction> GetActions(State s, Combat c) => new List<CardAction>
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

        public IEntry? GetUpgradeA(IJesterRequest request, out int cost)
        {
            if (Count <= 1)
            {
                cost = 0;
                return null;
            }

            cost = 7;
            return new HeatCostEntry(Count - 1);
        }

        public IEntry? GetUpgradeB(IJesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("heat");
        }
    }
}