using Jester.External;

namespace Jester.Generator.Strategy.Common;

using IJesterRequest = Jester.Api.IJesterApi.IJesterRequest;
using IEntry = Jester.Api.IJesterApi.IEntry;
using IProvider = Jester.Api.IJesterApi.IProvider;

public static class StrategyUtil
{
    public static IEnumerable<(double, IEntry)> GetOptionsFromProviders(IJesterRequest request, IEnumerable<IProvider> providers)
    {
        return providers
            .SelectMany(p => p.GetEntries(request))
            .Where(e => ModManifest.JesterApi.GetJesterUtil().InRange(request.MinCost, e.Item2.GetCost(), request.MaxCost))
            .Where(e => !e.Item2.Tags.Overlaps(request.Blacklist))
            .Where(e => ModManifest.JesterApi.GetJesterUtil().ContainsAll(e.Item2.Tags, request.Whitelist));
    }

    public static IEntry? GetRandomEntry(IJesterRequest request, IEnumerable<IProvider> providers, int actionCountLimit)
    {
        var entries = GetOptionsFromProviders(request, providers);

        var random = new WeightedRandom<IEntry>(
            entries
                .Where(e => e.Item2.GetActions(DB.fakeState, DB.fakeCombat).Count() <= actionCountLimit)
                .Select(e => new WeightedItem<IEntry>(e.Item1, e.Item2))
        );

        return random.WeightSum == 0 ? null : random.Next(request.Random);
    }

    public static IEnumerable<IEntry> FilterOptionBucketBiased(IJesterRequest request, IList<IEntry> entries, int buckets, bool expensive = false)
    {
        if (buckets == 1)
            return entries;

        var bucketDelta = request.MaxCost - request.MinCost;

        buckets = Math.Min(buckets, bucketDelta);
        
        var bucketTotal = buckets * (buckets + 1) / 2;
        var bucket = (int) Math.Floor((Math.Sqrt((request.Random.Next() % bucketTotal) * 8 + 1) - 1) / 2);

        if (!expensive)
            bucket = buckets - bucket - 1;

        do
        {
            var bucketMin = request.MinCost + 1 + bucketDelta * bucket / buckets;
            var bucketMax = request.MinCost + bucketDelta * (bucket + 1) / buckets;
            var result = entries.Where(e => ModManifest.JesterApi.GetJesterUtil().InRange(bucketMin, e.GetCost(), bucketMax)).ToList();
            
            if (result.Count > 0)
                return result;

            if (expensive)
                bucket++;
            else
                bucket--;
            
        } while (!expensive && bucket > 0 || expensive && bucket < buckets);

        return entries;
    }

    public static int PerformUpgrade(IJesterRequest request, ref int pts, Upgrade upDir, int upgradeLimit)
    {
        var points = pts;
        var entries = request.Entries;
        var upgradeCount = 0;
        var rng = new Rand((uint)(request.Random.seed + points + (int)upDir));
        var actionHardcap = ModManifest.Settings.ProfileBased.Current.ActionCap;
        var currentActions = entries.Select(e => e.GetActions(DB.fakeState, DB.fakeCombat).Count()).Sum();

        while (upgradeCount < upgradeLimit)
        {
            var points1 = points;
            var options = entries
                .SelectMany(e => e.GetUpgradeOptions(request, upDir)
                    .Select(u => Tuple.Create(u.Item1, e, u.Item2)
                    ).Where(d =>
                        d.Item3.GetActions(DB.fakeState, DB.fakeCombat).Count() -
                        d.Item2.GetActions(DB.fakeState, DB.fakeCombat).Count() + currentActions <= actionHardcap))
                .Where(d => d.Item3.GetCost() - d.Item2.GetCost() <= points1);
            
            if (!options.Any())
                break;
            
            var upgrade = new WeightedRandom<(IEntry, IEntry)>(options
                .Select(d => new WeightedItem<(IEntry, IEntry)>(d.Item1, (d.Item2, d.Item3))))
                .Next(rng);
            
            var idx = entries.IndexOf(upgrade.Item1);
            entries.RemoveAt(idx);
            entries.Insert(idx, upgrade.Item2);
            
            points -= upgrade.Item2.GetCost() - upgrade.Item1.GetCost();
            upgradeCount++;
        }

        pts = points;

        return upgradeCount;
    }
}