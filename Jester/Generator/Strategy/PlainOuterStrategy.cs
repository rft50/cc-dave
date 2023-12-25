using Jester.Generator.Provider;

namespace Jester.Generator.Strategy;

public class PlainOuterStrategy : IStrategy
{
    public JesterResult GenerateCard(JesterRequest request, List<IProvider> providers, int maxActions)
    {
        return JesterGenerator.CallInnerStrategy(request, providers, maxActions);
    }

    public double GetWeight(JesterRequest request) => 2;

    public StrategyCategory GetStrategyCategory() => StrategyCategory.Outer;
}