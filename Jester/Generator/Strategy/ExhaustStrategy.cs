using Jester.Generator.Provider;

namespace Jester.Generator.Strategy;

public class ExhaustStrategy : IStrategy
{
    public JesterResult GenerateCard(JesterRequest request, List<IProvider> providers, int maxActions)
    {
        request.CardData.exhaust = true;
        request.BasePoints += 20;
        return JesterGenerator.CallInnerStrategy(request, providers, maxActions);
    }

    public double GetWeight(JesterRequest request) => 1;
    public StrategyCategory GetStrategyCategory() => StrategyCategory.Outer;
}