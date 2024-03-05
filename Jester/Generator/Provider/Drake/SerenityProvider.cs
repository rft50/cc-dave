using Jester.Api;

namespace Jester.Generator.Provider.Drake;

public class SerenityProvider : IProvider
{
    public IList<IEntry> GetEntries(IJesterRequest request)
    {
        if (!ModManifest.JesterApi.HasCharacterFlag("heat")) return new List<IEntry>();
        
        var minCost = request.MinCost;
        var maxCost = request.MaxCost;

        return new List<IEntry>
            {
                new SerenityEntry()
            }.Where(e => ModManifest.JesterApi.GetJesterUtil().InRange(minCost, e.GetCost(), maxCost))
            .ToList();
    }
    
    public class SerenityEntry : IEntry
    {
        public ISet<string> Tags { get; } = new HashSet<string>
        {
            "status",
            "heat",
            "serenity",
            "cost",
            "weighted"
        };

        public int GetActionCount() => 1;

        public IList<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AStatus
            {
                status = Enum.Parse<Status>("serenity"),
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
            request.Blacklist.Add("heat");
        }
    }
}