﻿using Jester.Api;
using Jester.Generator.Provider;

namespace Jester.Generator.Strategy;

public class CostCardStrategy : IStrategy
{
    private static readonly HashSet<string> Cost = new()
    {
        "cost"
    };
    
    public IJesterResult GenerateCard(IJesterRequest request, IList<IProvider> providers, int maxActions)
    {
        var whitelist = request.Whitelist;
        request.Whitelist = Cost;
        request.MinCost = -request.BasePoints;
        request.MaxCost = request.MinCost / 2;
        var options = ModManifest.JesterApi.GetOptionsFromProvidersFiltered(request, providers);
        IEntry? cost = null;
        if (options.Count > 0)
        {
            cost = ModManifest.JesterApi.GetJesterUtil().GetRandom(options, request.Random);
            cost.AfterSelection(request);
            request.BasePoints -= cost.GetCost();
            maxActions -= cost.GetActionCount();
        }
        request.Whitelist = whitelist;

        var result = ModManifest.JesterApi.CallInnerStrategy(request, providers, maxActions);

        if (cost == null) return result;
        
        request.BasePoints += cost.GetCost();
        result.Entries.Add(cost);

        return result;
    }

    public double GetWeight(IJesterRequest request) => 1;

    public StrategyCategory GetStrategyCategory() => StrategyCategory.Outer;
}