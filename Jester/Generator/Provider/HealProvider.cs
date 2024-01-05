using Jester.Api;

namespace Jester.Generator.Provider;

public class HealProvider : IProvider
{
    public IList<IEntry> GetEntries(IJesterRequest request)
    {
        var cost = 35;
        if (!ModManifest.JesterApi.HasCardFlag("exhaust", request)) cost *= 2;

        if (!ModManifest.JesterApi.GetJesterUtil().InRange(request.MinCost, cost, request.MaxCost))
            return new List<IEntry>();

        return new List<IEntry>
        {
            new HealEntry(cost)
        };
    }
    
    public class HealEntry : IEntry
    {
        public int Cost { get; }

        public HealEntry(int cost)
        {
            Cost = cost;
        }
        
        public ISet<string> Tags =>
            new HashSet<string>
            {
                "defensive",
                "heal"
            };

        public int GetActionCount() => 1;

        public IList<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AHeal
            {
                healAmount = 1,
                targetPlayer = true,
                canRunAfterKill = true
            }
        };

        public int GetCost() => Cost;

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
            request.Blacklist.Add("heal");
        }
    }
}