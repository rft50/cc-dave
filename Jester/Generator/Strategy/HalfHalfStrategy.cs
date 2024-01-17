using Jester.Api;
using Jester.Generator.Provider;

namespace Jester.Generator.Strategy;

public class HalfHalfStrategy : IStrategy
{
    public IJesterResult GenerateCard(IJesterRequest request, IList<IProvider> providers)
    {
        var entries = request.Entries;
        var whitelist = request.Whitelist;
        var blacklist = request.Blacklist;
        var rng = request.Random;
        var points = request.BasePoints;
        var maxActions = request.ActionLimit;
        var actionCount = 0;
        
        // FIRST HALF

        request.MinCost = 1;
        
        if (request.FirstAction != null)
            whitelist.Add(request.FirstAction);

        IList<IEntry> options;

        do
        {
            request.MaxCost = points;
            options = ModManifest.JesterApi.GetOptionsFromProvidersFiltered(request, providers)
                .Where(e => e.GetActionCount() <= maxActions - actionCount)
                .ToList();;
            
            if (options.Count == 0) break;
            
            var option = ModManifest.JesterApi.GetJesterUtil().GetRandom(options, rng);
            option.AfterSelection(request);
            entries.Add(option);
            points -= option.GetCost();
            actionCount += option.GetActionCount();

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
            options = ModManifest.JesterApi.GetOptionsFromProvidersFiltered(request, providers)
                .Where(e => e.GetActionCount() <= maxActions - actionCount)
                .ToList();

            if (options.Count == 0) break;
            
            var option = ModManifest.JesterApi.GetJesterUtil().GetRandom(options, rng);
            option.AfterSelection(request);
            entries.Add(option);
            points -= option.GetCost();
            actionCount += option.GetActionCount();
        } while (true);
        
        // UPGRADES

        ModManifest.JesterApi.PerformUpgradeA(request, entries, ref points);

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