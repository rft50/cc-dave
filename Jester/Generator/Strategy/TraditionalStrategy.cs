using Jester.Generator.Provider;

namespace Jester.Generator.Strategy;

public class TraditionalStrategy : IStrategy
{
    public JesterResult GenerateCard(JesterRequest request, List<IProvider> providers)
    {
        var entries = request.Entries;
        var whitelist = request.Whitelist;
        var blacklist = request.Blacklist;
        var rng = new Random(request.Seed);
        request.Random = rng;
        var points = request.BasePoints;
        var actionCount = 0;

        // FIRST
        
        request.MinCost = points * 2 / 5;
        request.MaxCost = points * 4 / 5;
        if (request.FirstAction != null)
            whitelist.Add(request.FirstAction);
        
        var firstOptions = IStrategy.GetOptionsFromProvidersFiltered(request, providers);
        firstOptions = IStrategy.FilterOptionBucketBiased(request, firstOptions, 5);

        var first = Util.GetRandom(firstOptions, rng);
        first.AfterSelection(request);
        entries.Add(first);

        points -= first.GetCost();
        actionCount += first.GetActionCount();
        if (request.FirstAction != null)
            whitelist.Remove(request.FirstAction);
        
        // FILL BOARD
        
        request.MinCost = points / 3;
        request.MaxCost = points * 2 / 3;
        
        var options = IStrategy.GetOptionsFromProvidersFiltered(request, providers);

        do
        {
            options = options.Where(e => e.GetActionCount() <= 5 - actionCount).ToList();

            if (options.Count == 0)
            {
                if (request.MinCost == 0)
                    break;

                request.MinCost = 0;
                request.MaxCost = points;
            }
            else
            {
                var entry = Util.GetRandom(options, rng);
                entry.AfterSelection(request);
                entries.Add(entry);
                points -= entry.GetCost();
                
                request.MinCost = points / 3;
                request.MaxCost = points * 2 / 3;
            }

            options = IStrategy.GetOptionsFromProvidersFiltered(request, providers);
        } while (options.Count != 0);
        
        // SPEND FINAL POINTS WITH UPGRADE A

        var upgradeOptions = entries.Select(e =>
        {
            var result = e.GetUpgradeA(request, out var cost);
            return Tuple.Create(e, result, cost);
        }).Where(e => e.Item2 != null && e.Item3 <= points)
            .Select(e => e as Tuple<IEntry, IEntry, int>).ToList();

        while (upgradeOptions.Any())
        {
            var upgrade = Util.GetRandom(upgradeOptions, rng);
            upgradeOptions.Remove(upgrade);

            var idx = entries.IndexOf(upgrade.Item1);
            entries.RemoveAt(idx);
            entries.Insert(idx, upgrade.Item2);
            
            points -= upgrade.Item3;
            var newResult = upgrade.Item2.GetUpgradeA(request, out var cost);
            if (newResult != null && cost <= points)
                upgradeOptions.Add(Tuple.Create(upgrade.Item2, newResult, cost));
        }

        return new JesterResult
        {
            Entries = entries,
            CardData = request.CardData
        };
    }
}