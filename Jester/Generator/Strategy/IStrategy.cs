using Jester.Generator.Provider;

namespace Jester.Generator.Strategy;

public interface IStrategy
{
    public JesterResult GenerateCard(JesterRequest request, List<IProvider> providers, int maxActions);

    public double GetWeight(JesterRequest request);

    public StrategyCategory GetStrategyCategory();

    protected static List<IEntry> GetOptionsFromProviders(JesterRequest request, IEnumerable<IProvider> providers)
    {
        return providers.SelectMany(p => p.GetEntries(request)).ToList();
    }

    protected static List<IEntry> GetOptionsFromProvidersFiltered(JesterRequest request, IEnumerable<IProvider> providers)
    {
        return providers.SelectMany(p => p.GetEntries(request))
            .Where(e => !e.Tags.Overlaps(request.Blacklist))
            .Where(e => Util.ContainsAll(e.Tags, request.Whitelist)).ToList();
    }

    protected static List<IEntry> FilterOptionBucketBiased(JesterRequest request, List<IEntry> entries, int buckets, bool expensive = false)
    {
        if (buckets == 1)
            return entries;

        var bucketDelta = request.MaxCost - request.MinCost;

        buckets = Math.Min(buckets, bucketDelta);
        
        var bucketTotal = buckets * (buckets + 1) / 2;
        var bucket = (int) Math.Floor((Math.Sqrt(request.Random.Next(bucketTotal) * 8 + 1) - 1) / 2);

        if (!expensive)
            bucket = buckets - bucket - 1;

        do
        {
            var bucketMin = request.MinCost + 1 + bucketDelta * bucket / buckets;
            var bucketMax = request.MinCost + bucketDelta * (bucket + 1) / buckets;
            var result = entries.Where(e => Util.InRange(bucketMin, e.GetCost(), bucketMax)).ToList();
            
            if (result.Count > 0)
                return result;

            if (expensive)
                bucket++;
            else
                bucket--;
            
        } while (!expensive && bucket > 0 || expensive && bucket < buckets);

        return entries;
    }

    protected static int PerformUpgradeA(JesterRequest request, List<IEntry> entries, ref int pts)
    {
        var points = pts;
        var upgradeCount = 0;
        var rng = new Random(request.Seed);
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
            upgradeCount++;

            var idx = entries.IndexOf(upgrade.Item1);
            entries.RemoveAt(idx);
            entries.Insert(idx, upgrade.Item2);
            
            points -= upgrade.Item3;
            var newResult = upgrade.Item2.GetUpgradeA(request, out var cost);
            if (newResult != null && cost <= points)
                upgradeOptions.Add(Tuple.Create(upgrade.Item2, newResult, cost));
        }

        pts = points;
        return upgradeCount;
    }
}

public enum StrategyCategory
{
    Full,
    Outer,
    Inner
}