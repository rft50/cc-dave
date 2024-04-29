using Jester.Api;

namespace Jester.Generator.Strategy.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IJesterResult = IJesterApi.IJesterResult;
using IProvider = IJesterApi.IProvider;
using IStrategy = IJesterApi.IStrategy;
using StrategyCategory = IJesterApi.StrategyCategory;

public class ExhaustCostCardStrategy : IStrategy
{
    public IJesterResult GenerateCard(IJesterRequest request, IEnumerable<IProvider> providers)
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