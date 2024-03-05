using Jester.Api;

namespace Jester.Generator.Provider.Books;

public class ShardProvider : IProvider
{
    public IList<IEntry> GetEntries(IJesterRequest request)
    {
        if (!ModManifest.JesterApi.HasCharacterFlag("shard")) return new List<IEntry>();
        
        var minCost = request.MinCost;
        var maxCost = request.MaxCost;

        return new List<IEntry>
            {
                new ShardEntry(1),
                new ShardEntry(2),
                new ShardEntry(3)
            }.Where(e => ModManifest.JesterApi.GetJesterUtil().InRange(minCost, e.GetCost(), maxCost))
            .ToList();
    }

    public class ShardEntry : IEntry
    {
        public int Count { get; set; }

        public ShardEntry()
        {
            
        }

        public ShardEntry(int count)
        {
            Count = count;
        }
        public ISet<string> Tags { get; } = new HashSet<string>
        {
            "status",
            "shard",
            "weighted"
        };

        public int GetActionCount() => 1;

        public IList<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AStatus
            {
                status = Enum.Parse<Status>("shard"),
                statusAmount = Count,
                targetPlayer = true
            }
        };

        public int GetCost() => 5 * Count;

        public IEntry? GetUpgradeA(IJesterRequest request, out int cost)
        {
            if (Count >= 3)
            {
                cost = 0;
                return null;
            }

            cost = 5;
            return new ShardEntry(Count + 1);
        }

        public IEntry? GetUpgradeB(IJesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("shard");
        }
    }
}