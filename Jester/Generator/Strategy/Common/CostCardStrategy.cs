using Jester.Api;

namespace Jester.Generator.Strategy.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IJesterResult = IJesterApi.IJesterResult;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;
using IStrategy = IJesterApi.IStrategy;
using StrategyCategory = IJesterApi.StrategyCategory;

public class CostCardStrategy : IStrategy
{
    private static readonly HashSet<string> Cost = new()
    {
        "cost"
    };
    
    public IJesterResult GenerateCard(IJesterRequest request, IEnumerable<IProvider> providers)
    {
        var whitelist = request.Whitelist;
        request.Whitelist = Cost;
        request.MinCost = -request.BasePoints;
        request.MaxCost = request.MinCost / 4;
        var cost = ModManifest.JesterApi.GetRandomEntry(request, providers, request.ActionLimit);
        if (cost != null)
        {
            cost.AfterSelection(request);
            request.BasePoints -= cost.GetCost();
            request.ActionLimit -= cost.GetActions(DB.fakeState, DB.fakeCombat).Count();
        }
        request.Whitelist = whitelist;

        var result = ModManifest.JesterApi.CallInnerStrategy(request, providers);

        if (cost == null) return result;
        
        request.BasePoints += cost.GetCost();
        result.Entries.Add(cost);

        return result;
    }

    public double GetWeight(IJesterRequest request) => 1;

    public StrategyCategory GetStrategyCategory() => StrategyCategory.Outer;
}