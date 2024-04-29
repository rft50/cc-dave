using Jester.Api;

namespace Jester.Generator.Provider.Books;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class ShardProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        if (!ModManifest.JesterApi.HasCharacterFlag("shard")) return new List<(double, IEntry)>();
        return Enumerable.Range(1, 3)
            .Select(i => (1.3, new ShardEntry
            {
                Count = i
            } as IEntry));
    }

    private class ShardEntry : IEntry
    {
        public int Count { get; set; }
        
        public IReadOnlySet<string> Tags { get; } = new HashSet<string>
        {
            "status",
            "shard"
        };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AStatus
            {
                status = Enum.Parse<Status>("shard"),
                statusAmount = Count,
                targetPlayer = true
            }
        };

        public int GetCost() => 5 * Count;

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            if (Count >= 3) return new List<(double, IEntry)>();
            return new List<(double, IEntry)>
            {
                (1, new ShardEntry
                {
                    Count = Count + 1
                })
            };
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("shard");
        }
    }
}