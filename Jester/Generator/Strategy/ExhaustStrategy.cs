using Jester.Api;
using Jester.Generator.Provider;

namespace Jester.Generator.Strategy;

public class ExhaustStrategy : IStrategy
{
    public IJesterResult GenerateCard(IJesterRequest request, IList<IProvider> providers, int maxActions)
    {
        var data = request.CardData;
        data.exhaust = true;
        request.CardData = data;
        request.BasePoints += 20;
        return ModManifest.JesterApi.CallInnerStrategy(request, providers, maxActions);
    }

    public double GetWeight(IJesterRequest request) => 1;
    public StrategyCategory GetStrategyCategory() => StrategyCategory.Outer;
}