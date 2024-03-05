using Jester.Api;

namespace Jester.Generator.Provider.Common;

public class DrawProvider : IProvider
{
    public IList<IEntry> GetEntries(IJesterRequest request)
    {
        if (request.CardData.cost == 0 && !ModManifest.JesterApi.HasCardFlag("exhaust", request)) return new List<IEntry>();
        
        var minCost = request.MinCost;
        var maxCost = request.MaxCost;

        return new List<IEntry>
            {
                new DrawEntry(1),
                new DrawEntry(2),
                new DrawEntry(3)
            }.Where(e => ModManifest.JesterApi.GetJesterUtil().InRange(minCost, e.GetCost(), maxCost))
            .ToList();
    }
    
    public class DrawEntry : IEntry
    {
        public int Count { get; set; }

        public DrawEntry()
        {
            
        }

        public DrawEntry(int count)
        {
            Count = count;
        }
        public ISet<string> Tags { get; } = new HashSet<string>
        {
            "utility",
            "draw"
        };

        public int GetActionCount() => 1;

        public IList<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new ADrawCard
            {
                count = Count
            }
        };

        public int GetCost() => 8 * Count;

        public IEntry? GetUpgradeA(IJesterRequest request, out int cost)
        {
            if (Count >= 7)
            {
                cost = 0;
                return null;
            }
            cost = 8;
            return new DrawEntry(Count + 1);
        }

        public IEntry? GetUpgradeB(IJesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("draw");
        }
    }
}