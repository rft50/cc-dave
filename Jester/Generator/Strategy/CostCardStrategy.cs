using Jester.Api;
using Jester.Generator.Provider;

namespace Jester.Generator.Strategy;

public class CostCardStrategy : IStrategy
{
    public IJesterResult GenerateCard(IJesterRequest request, IList<IProvider> providers, int maxActions)
    {
        request.Whitelist.Add("cost");
        request.MinCost = -request.BasePoints;
        request.MaxCost = request.MinCost / 2;
        var cost = ModManifest.JesterApi.GetJesterUtil().GetRandom(ModManifest.JesterApi.GetOptionsFromProvidersFiltered(request, providers), request.Random);
        request.Whitelist.Remove("cost");

        request.BasePoints -= cost.GetCost();
        var result = ModManifest.JesterApi.CallInnerStrategy(request, providers, maxActions - cost.GetActionCount());
        request.BasePoints += cost.GetCost();
        
        result.Entries.Add(cost);
        return result;
    }

    public double GetWeight(IJesterRequest request) => 1;

    public StrategyCategory GetStrategyCategory() => StrategyCategory.Outer;
}