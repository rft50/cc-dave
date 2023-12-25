using Jester.Generator.Provider;

namespace Jester.Generator.Strategy;

public class CostCardStrategy : IStrategy
{
    public JesterResult GenerateCard(JesterRequest request, List<IProvider> providers, int maxActions)
    {
        request.Whitelist.Add("cost");
        request.MinCost = -request.BasePoints;
        request.MaxCost = request.MinCost / 2;
        var cost = Util.GetRandom(IStrategy.GetOptionsFromProvidersFiltered(request, providers), request.Random);
        request.Whitelist.Remove("cost");

        request.BasePoints -= cost.GetCost();
        var result = JesterGenerator.CallInnerStrategy(request, providers, maxActions - cost.GetActionCount());
        request.BasePoints += cost.GetCost();
        
        result.Entries.Add(cost);
        return result;
    }

    public double GetWeight(JesterRequest request) => 1;

    public StrategyCategory GetStrategyCategory() => StrategyCategory.Outer;
}