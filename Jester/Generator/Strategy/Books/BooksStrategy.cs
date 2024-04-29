using Jester.Api;
using Jester.Generator.Provider.Books;
using Microsoft.Extensions.Logging;

namespace Jester.Generator.Strategy.Books;

using IJesterRequest = IJesterApi.IJesterRequest;
using IJesterResult = IJesterApi.IJesterResult;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;
using IStrategy = IJesterApi.IStrategy;
using StrategyCategory = IJesterApi.StrategyCategory;

public class BooksStrategy : IStrategy
{
    public IJesterResult GenerateCard(IJesterRequest request, IEnumerable<IProvider> providers)
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

        while (actionCount < maxActions)
        {
            actionCount++;
            
            if (actionCount % 2 == 0)
                points += -ShardCost(shardDistribution[actionCount / 2 - 1]);
            request.MaxCost = points;
            var option = ModManifest.JesterApi.GetRandomEntry(request, providers, 1);
            
            if (option == null) break;
            
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

        ModManifest.JesterApi.PerformUpgrade(request, ref points, Upgrade.None);

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

    private class ShardCostEntry : IEntry
    {
        public int Count { get; set; }
        public IEntry Entry { get; set; } = null!;

        public IReadOnlySet<string> Tags => Entry.Tags;

        public IEnumerable<CardAction> GetActions(State s, Combat c)
        {
            var actions = Entry.GetActions(s, c).ToList();
            actions[0].shardcost = Count;
            return actions;
        }

        public int GetCost()
        {
            return Entry.GetCost() + ShardCost(Count); 
        }

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            if (Count <= 1) return WrapUpgradeOptions(Entry.GetUpgradeOptions(request, upDir));
            return WrapUpgradeOptions(
                    Entry
                    .GetUpgradeOptions(request, upDir)
                    )
                .Append((0.5, new ShardCostEntry
                {
                    Count = Count - 1,
                    Entry = Entry
                }));
        }

        private IEnumerable<(double, IEntry)> WrapUpgradeOptions(IEnumerable<(double, IEntry)> options)
        {
            return options.Select(e => (e.Item1, new ShardCostEntry
            {
                Count = Count,
                Entry = e.Item2
            } as IEntry));
        }

        public void AfterSelection(IJesterRequest request)
        {
            Entry.AfterSelection(request);
        }
    }
}