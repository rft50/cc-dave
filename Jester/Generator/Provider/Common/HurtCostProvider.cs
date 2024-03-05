using Jester.Api;

namespace Jester.Generator.Provider.Common;

public class HurtCostProvider : IProvider
{
    public IList<IEntry> GetEntries(IJesterRequest request)
    {
        if (!request.Whitelist.Contains("cost")) return new List<IEntry>();
        
        var minCost = request.MinCost;
        var maxCost = request.MaxCost;
        
        return new List<IEntry>
        {
            
        }.Where(e => ModManifest.JesterApi.GetJesterUtil().InRange(minCost, e.GetCost(), maxCost))
        .ToList();
    }
    
    public class HurtEntry : IEntry
    {
        public ISet<string> Tags
        {
            get => new HashSet<string>
            {
                "cost",
                "hurt"
            };
            
        }

        public int GetActionCount() => 1;

        public IList<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AHurt
            {
                targetPlayer = true,
                hurtShieldsFirst = false
            }
        };

        public int GetCost() => -30;

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
        }
    }
}