using Jester.Api;

namespace Jester.Generator.Provider.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class HurtCostProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        if (!request.Whitelist.Contains("cost")) return new List<(double, IEntry)>();

        return new List<(double, IEntry)>
        {
            (1, new HurtEntry())
        };
    }
    
    private class HurtEntry : IEntry
    {
        public IReadOnlySet<string> Tags =>
            new HashSet<string>
            {
                "cost",
                "hurt"
            };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AHurt
            {
                targetPlayer = true,
                hurtShieldsFirst = false
            }
        };

        public int GetCost() => -30;

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            return new List<(double, IEntry)>();
        }

        public void AfterSelection(IJesterRequest request)
        {
        }
    }
}