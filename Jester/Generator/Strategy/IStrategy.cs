using Jester.Generator.Provider;

namespace Jester.Generator.Strategy;

public interface IStrategy
{
    public JesterResult GenerateCard(JesterRequest request, List<IProvider> providers);

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
}