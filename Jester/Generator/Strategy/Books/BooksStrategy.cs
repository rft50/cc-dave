using Jester.Api;
using Jester.Generator.Provider.Books;
using Microsoft.Extensions.Logging;

namespace Jester.Generator.Strategy.Books;

public class BooksStrategy : IStrategy
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
        var shardDistribution = CreateShardDistribution(maxActions / 2, rng.NextInt() % 5 + 1, 3, rng);
        
        // INITIAL ENTRIES

        request.MinCost = 1;
        blacklist.Add("shard");

        IList<IEntry> options;

        while (actionCount < maxActions)
        {
            actionCount++;
            
            if (actionCount % 2 == 0)
                points += -ShardCost(shardDistribution[actionCount / 2 - 1]);
            request.MaxCost = points;
            options = ModManifest.JesterApi.GetOptionsFromProvidersWeighted(request, providers)
                .Where(e => e.GetActionCount() == 1)
                .ToList();
            
            if (options.Count == 0) break;
            
            var option = ModManifest.JesterApi.GetJesterUtil().GetRandom(options, rng);
            
            if (actionCount % 2 == 0)
                option = new ShardCostEntry
                {
                    Count = shardDistribution[actionCount / 2 - 1],
                    Entry = option
                };
            option.AfterSelection(request);
            entries.Add(option);
            points -= option.GetCost();
        }

        ModManifest.JesterApi.PerformUpgradeA(request, entries, ref points);

        return new JesterResult
        {
            Entries = entries,
            CardData = request.CardData,
            SparePoints = points
        };
    }

    private static int[] CreateShardDistribution(int buckets, int points, int limitPerBucket, Rand rng)
    {
        var data = new int[buckets];
        var options = Enumerable.Repeat(Enumerable.Range(0, buckets), limitPerBucket)
            .SelectMany(e => e)
            .ToList();
        ModManifest.JesterApi.GetJesterUtil().Shuffle(options, rng);
        foreach (var i in options.Take(points))
        {
            data[i]++;
        }
        
        return data;
    }

    private static int ShardCost(int x) // -5, -11, -18, -27, -37, etc
    {
        return -(x + 9) * x / 2;
    }

    public double GetWeight(IJesterRequest request) => ModManifest.JesterApi.HasCharacterFlag("shard") ? 1 : 0;

    public StrategyCategory GetStrategyCategory() => StrategyCategory.Full;

    public class ShardCostEntry : IEntry
    {
        public int Count { get; set; }
        public IEntry Entry { get; set; } = null!;

        public ISet<string> Tags => Entry.Tags;
        public int GetActionCount() => Entry.GetActionCount();

        public IList<CardAction> GetActions(State s, Combat c)
        {
            var actions = Entry.GetActions(s, c);
            actions[0].shardcost = Count;
            return actions;
        }

        public int GetCost()
        {
            return Entry.GetCost() + ShardCost(Count); 
        }

        public IEntry? GetUpgradeA(IJesterRequest request, out int cost)
        {
            var inner = Entry.GetUpgradeA(request, out cost);
            if (inner != null) return new ShardCostEntry
                {
                    Count = Count,
                    Entry = inner
                };
            if (Count == 1)
            {
                cost = 0;
                return null;
            }

            cost = ShardCost(Count) - ShardCost(Count - 1);
            return new ShardCostEntry
            {
                Count = Count - 1,
                Entry = Entry
            };
        }

        public IEntry? GetUpgradeB(IJesterRequest request, out int cost)
        {
            var inner = Entry.GetUpgradeB(request, out cost);
            if (inner != null) return new ShardCostEntry
                {
                    Count = Count,
                    Entry = inner
                };
            if (Count == 1)
            {
                cost = 0;
                return null;
            }

            cost = ShardCost(Count) - ShardCost(Count - 1);
            return new ShardCostEntry
            {
                Count = Count - 1,
                Entry = Entry
            };
        }

        public void AfterSelection(IJesterRequest request)
        {
            Entry.AfterSelection(request);
        }
    }
}