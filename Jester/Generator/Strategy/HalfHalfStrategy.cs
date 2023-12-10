using Jester.Generator.Provider;

namespace Jester.Generator.Strategy;

public class HalfHalfStrategy : IStrategy
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
        
        // FIRST HALF

        request.MinCost = 1;
        
        if (request.FirstAction != null)
            whitelist.Add(request.FirstAction);

        List<IEntry> options;

        do
        {
            request.MaxCost = points;
            options = IStrategy.GetOptionsFromProvidersFiltered(request, providers);
            var option = Util.GetRandom(options, rng);
            option.AfterSelection(request);
            entries.Add(option);
            points -= option.GetCost();
            actionCount += option.GetActionCount();

            if (options.Count == 0) break;
        } while (points > request.BasePoints / 2);

        if (request.FirstAction != null)
        {
            whitelist.Remove(request.FirstAction);
            blacklist.Add(request.FirstAction);
        }

        do
        {
            request.MaxCost = points;
            options = IStrategy.GetOptionsFromProvidersFiltered(request, providers)
                .Where(e => e.GetActionCount() <= 5 - actionCount)
                .ToList();

            if (options.Count == 0) break;
            
            var option = Util.GetRandom(options, rng);
            option.AfterSelection(request);
            entries.Add(option);
            points -= option.GetCost();
            actionCount += option.GetActionCount();
        } while (true);

        return new JesterResult
        {
            Entries = entries,
            CardData = request.CardData
        };
    }
}