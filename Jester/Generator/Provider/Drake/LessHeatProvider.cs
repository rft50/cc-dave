using Jester.Api;

namespace Jester.Generator.Provider.Drake;

public class LessHeatProvider : IProvider
{
    public IList<IEntry> GetEntries(IJesterRequest request)
    {
        if (!ModManifest.JesterApi.HasCharacterFlag("heat")) return new List<IEntry>();
        
        var minCost = request.MinCost;
        var maxCost = request.MaxCost;

        return new List<IEntry>
            {
                new LessHeatEntry(1),
                new LessHeatEntry(2),
                new LessHeatEntry(3)
            }.Where(e => ModManifest.JesterApi.GetJesterUtil().InRange(minCost, e.GetCost(), maxCost))
            .ToList();
    }

    public class LessHeatEntry : IEntry
    {
        private static int[] _costs = { 7, 15, 24 };
        
        public int Count { get; set; }

        public LessHeatEntry()
        {
        }

        public LessHeatEntry(int count)
        {
            Count = count;
        }
        public ISet<string> Tags { get; } = new HashSet<string>
        {
            "status",
            "heat",
            "weighted"
        };

        public int GetActionCount() => 1;

        public IList<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AStatus
            {
                status = Enum.Parse<Status>("heat"),
                statusAmount = -Count,
                targetPlayer = true
            }
        };

        public int GetCost()
        {
            return _costs[Count - 1];
        }

        public IEntry? GetUpgradeA(IJesterRequest request, out int cost)
        {
            if (Count >= _costs.Length)
            {
                cost = 0;
                return null;
            }
            var newEntry = new LessHeatEntry(Count + 1);
            cost = newEntry.GetCost() - GetCost();
            return newEntry;
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