﻿using Jester.Api;

namespace Jester.Generator.Strategy.Common;

public static class StrategyUtil
{
    public static IList<IEntry> GetOptionsFromProviders(IJesterRequest request, IEnumerable<IProvider> providers)
    {
        return providers.SelectMany(p => p.GetEntries(request)).ToList();
    }

    public static IList<IEntry> GetOptionsFromProvidersFiltered(IJesterRequest request, IEnumerable<IProvider> providers)
    {
        return providers.SelectMany(p => p.GetEntries(request))
            .Where(e => !e.Tags.Overlaps(request.Blacklist))
            .Where(e => ModManifest.JesterApi.GetJesterUtil().ContainsAll(e.Tags, request.Whitelist)).ToList();
    }

    public static IList<IEntry> GetOptionsFromProvidersWeighted(IJesterRequest request, IEnumerable<IProvider> providers)
    {
        var entries = GetOptionsFromProvidersFiltered(request, providers);
        var weighted = entries.Where(e => e.Tags.Contains("weighted")).ToList();

        if (weighted.Count != 0 && request.Random.Next() < 0.2)
            return weighted;
        return entries;
    }

    public static IList<IEntry> FilterOptionBucketBiased(IJesterRequest request, IList<IEntry> entries, int buckets, bool expensive = false)
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

    public static int PerformUpgradeA(IJesterRequest request, IList<IEntry> entries, ref int pts, int upgradeLimit)
    {
        var points = pts;
        var upgradeCount = 0;
        var rng = new Rand((uint)(request.Seed + pts));
        var upgradeOptions = entries.Select(e =>
            {
                var result = e.GetUpgradeA(request, out var cost);
                return Tuple.Create(e, result, cost);
            }).Where(e => e.Item2 != null && e.Item3 <= points)
            .Select(e => e as Tuple<IEntry, IEntry, int>).ToList();

        while (upgradeOptions.Any() && upgradeCount < upgradeLimit)
        {
            var upgrade = ModManifest.JesterApi.GetJesterUtil().GetRandom(upgradeOptions, rng);
            upgradeOptions.Remove(upgrade);
            if (upgrade.Item3 > points)
                continue;
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

    public static int PerformUpgradeB(IJesterRequest request, IList<IEntry> entries, ref int pts, int upgradeLimit)
    {
        var points = pts;
        var upgradeCount = 0;
        var rng = new Rand((uint)(request.Seed + pts));
        var upgradeOptions = entries.Select(e =>
            {
                var result = e.GetUpgradeB(request, out var cost);
                return Tuple.Create(e, result, cost);
            }).Where(e => e.Item2 != null && e.Item3 <= points)
            .Select(e => e as Tuple<IEntry, IEntry, int>).ToList();

        while (upgradeOptions.Any() && upgradeCount < upgradeLimit)
        {
            var upgrade = ModManifest.JesterApi.GetJesterUtil().GetRandom(upgradeOptions, rng);
            upgradeOptions.Remove(upgrade);
            if (upgrade.Item3 > points)
                continue;
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
        
        var aUps = PerformUpgradeA(request, entries, ref pts, upgradeLimit - upgradeCount);
        upgradeCount += aUps;
        
        return upgradeCount;
    }
}