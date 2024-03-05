using Jester.Api;

namespace Jester.Generator.Strategy.Common;

public class CostCardStrategy : IStrategy
{
    private static readonly HashSet<string> Cost = new()
    {
        "cost"
    };
    
    public IJesterResult GenerateCard(IJesterRequest request, IList<IProvider> providers)
    {
        var whitelist = request.Whitelist;
        request.Whitelist = Cost;
        request.MinCost = -request.BasePoints;
        request.MaxCost = request.MinCost / 4;
        var options = ModManifest.JesterApi.GetOptionsFromProvidersWeighted(request, providers);
        IEntry? cost = null;
        if (options.Count > 0)
        {
            cost = ModManifest.JesterApi.GetJesterUtil().GetRandom(options, request.Random);
            cost.AfterSelection(request);
            request.BasePoints -= cost.GetCost();
            request.ActionLimit -= cost.GetActionCount();
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