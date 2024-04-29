using Jester.Api;

namespace Jester.Generator.Strategy.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IJesterResult = IJesterApi.IJesterResult;
using IProvider = IJesterApi.IProvider;
using IStrategy = IJesterApi.IStrategy;
using StrategyCategory = IJesterApi.StrategyCategory;

public class PlainOuterStrategy : IStrategy
{
    public IJesterResult GenerateCard(IJesterRequest request, IEnumerable<IProvider> providers)
    {
        return ModManifest.JesterApi.CallInnerStrategy(request, providers);
    }

    public double GetWeight(IJesterRequest request) => 2;

    public StrategyCategory GetStrategyCategory() => StrategyCategory.Outer;
}