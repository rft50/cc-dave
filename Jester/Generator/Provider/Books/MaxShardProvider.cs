using Jester.Api;

namespace Jester.Generator.Provider.Books;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class MaxShardProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        if (!ModManifest.JesterApi.HasCharacterFlag("shard")) return new List<(double, IEntry)>();
        if (request.CardMeta.rarity != Rarity.uncommon && request.CardMeta.rarity != Rarity.rare) return new List<(double, IEntry)>();

        return new List<(double, IEntry)>
        {
            (4, new MaxShardEntry())
        };
    }
    
    private class MaxShardEntry : IEntry
    {
        public IReadOnlySet<string> Tags { get; } = new HashSet<string>
        {
            "status",
            "utility",
            "maxShard",
            "mustExhaust"
        };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AStatus
            {
                status = Enum.Parse<Status>("maxShard"),
                statusAmount = 1,
                targetPlayer = true
            }
        };

        public int GetCost() => 15;

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            return new List<(double, IEntry)>();
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("maxShard");
        }
    }
}