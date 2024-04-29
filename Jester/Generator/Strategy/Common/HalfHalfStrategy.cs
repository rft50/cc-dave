using Jester.Api;

namespace Jester.Generator.Strategy.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IJesterResult = IJesterApi.IJesterResult;
using IProvider = IJesterApi.IProvider;
using IStrategy = IJesterApi.IStrategy;
using StrategyCategory = IJesterApi.StrategyCategory;

public class HalfHalfStrategy : IStrategy
{
    public IJesterResult GenerateCard(IJesterRequest request, IEnumerable<IProvider> providers)
    {
        var entries = request.Entries;
        var whitelist = request.Whitelist;
        var blacklist = request.Blacklist;
        var points = request.BasePoints;
        var maxActions = request.ActionLimit;
        var actionCount = 0;
        
        // FIRST HALF

        request.MinCost = 1;
        
        if (request.FirstAction != null)
            whitelist.Add(request.FirstAction);

        do
        {
            request.MaxCost = points;
            var option = ModManifest.JesterApi.GetRandomEntry(request, providers, maxActions - actionCount);
            
            if (option == null) break;
            
            option.AfterSelection(request);
            entries.Add(option);
            points -= option.GetCost();
            actionCount += option.GetActions(DB.fakeState, DB.fakeCombat).Count();

        } while (points > request.BasePoints / 2);

        if (request.FirstAction != null)
        {
            whitelist.Remove(request.FirstAction);
            blacklist.Add(request.FirstAction);
        }
        
        // SECOND HALF

        do
        {
            request.MaxCost = points;
            var option = ModManifest.JesterApi.GetRandomEntry(request, providers, maxActions - actionCount);

            if (option == null) break;
            
            option.AfterSelection(request);
            entries.Add(option);
            points -= option.GetCost();
            actionCount += option.GetActions(DB.fakeState, DB.fakeCombat).Count();
        } while (true);
        
        // UPGRADES

        ModManifest.JesterApi.PerformUpgrade(request, ref points, Upgrade.None);

        return new JesterResult
        {
            Entries = entries,
            CardData = request.CardData,
            SparePoints = points
        };
    }

    public double GetWeight(IJesterRequest request) => 2;
    public StrategyCategory GetStrategyCategory() => StrategyCategory.Inner;
}