﻿using Jester.Api;
using Jester.Generator.Provider;

namespace Jester.Generator.Strategy;

public class PlainOuterStrategy : IStrategy
{
    public IJesterResult GenerateCard(IJesterRequest request, IList<IProvider> providers)
    {
        return ModManifest.JesterApi.CallInnerStrategy(request, providers);
    }

    public double GetWeight(IJesterRequest request) => 2;

    public StrategyCategory GetStrategyCategory() => StrategyCategory.Outer;
}