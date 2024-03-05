using Jester.Api;

namespace Jester.Generator.Provider.Books;

public class MaxShardProvider : IProvider
{
    public IList<IEntry> GetEntries(IJesterRequest request)
    {
        if (!ModManifest.JesterApi.HasCharacterFlag("shard")) return new List<IEntry>();
        if (request.CardMeta.rarity != Rarity.uncommon && request.CardMeta.rarity != Rarity.rare) return new List<IEntry>();
        
        var minCost = request.MinCost;
        var maxCost = request.MaxCost;

        return new List<IEntry>
            {
                new MaxShardEntry()
            }.Where(e => ModManifest.JesterApi.GetJesterUtil().InRange(minCost, e.GetCost(), maxCost))
            .ToList();
    }
    
    public class MaxShardEntry : IEntry
    {
        public ISet<string> Tags { get; } = new HashSet<string>
        {
            "status",
            "utility",
            "maxShard",
            "mustExhaust",
            "weighted"
        };

        public int GetActionCount() => 1;

        public IList<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AStatus
            {
                status = Enum.Parse<Status>("maxShard"),
                statusAmount = 1,
                targetPlayer = true
            }
        };

        public int GetCost() => 15;

        public IEntry? GetUpgradeA(IJesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public IEntry? GetUpgradeB(IJesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("maxShard");
        }
    }
}