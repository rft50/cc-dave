﻿using Jester.Api;
using Jester.Generator.Provider;

namespace Jester.Generator.Strategy;

public class ExhaustCostCardStrategy : IStrategy
{
    public IJesterResult GenerateCard(IJesterRequest request, IList<IProvider> providers)
    {
        var data = request.CardData;
        data.exhaust = true;
        request.CardData = data;
        request.BasePoints += 20;
        
        return new CostCardStrategy().GenerateCard(request, providers);
    }

    public double GetWeight(IJesterRequest request) => ModManifest.JesterApi.HasCardFlag("singleUse", request) ? 0 : 1;
    public StrategyCategory GetStrategyCategory() => StrategyCategory.Outer;
}