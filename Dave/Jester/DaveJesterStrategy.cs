using System.Collections;
using Dave.Actions;
using Dave.External;

namespace Dave.Jester;

public class DaveJesterStrategy : IJesterApi.IStrategy
{
    public IJesterApi.IJesterResult GenerateCard(IJesterApi.IJesterRequest request, IEnumerable<IJesterApi.IProvider> providers)
    {
        var guid = Guid.NewGuid();
        var entries = new List<IJesterApi.IEntry>();
        var rng = request.Random;
        var points = request.BasePoints;
        var distribution = GenerateDistribution(request.ActionLimit, rng);

        while (distribution.Count > 0)
        {
            var color = distribution[0];
            distribution.Remove(color);

            if (color == RedBlack.Blank)
                request.MaxCost = points / 2;
            else
                request.MaxCost = points * 4 / 3;

            var blacklisted = color switch
            {
                RedBlack.Red => "defensive",
                RedBlack.Black => "offensive",
                _ => null
            };
            
            if (blacklisted != null)
                request.Blacklist.Add(blacklisted);

            var option = ModEntry.Instance.JesterApi!.GetRandomEntry(request, providers, distribution.Count);
            
            if (blacklisted != null)
                request.Blacklist.Remove(blacklisted);

            if (option == null) continue;

            if (color != RedBlack.Blank)
                option = new RedBlackEntry
                {
                    Guid = guid,
                    Inner = option,
                    IsRed = color == RedBlack.Red
                };
            
            option.AfterSelection(request);
            entries.Add(option);
            points -= option.GetCost();

            var count = option.GetActions(DB.fakeState, DB.fakeCombat).Count();

            for (var i = 1; i < count; i++)
            {
                if (!distribution.Remove(color))
                    distribution.RemoveAt(0);
            }
        }
        
        entries.Insert(0, new SetupEntry {Guid = guid});

        var result = ModEntry.Instance.JesterApi!.NewJesterResult();
        result.Entries = entries;
        result.CardData = request.CardData;

        return result;
    }

    public double GetWeight(IJesterApi.IJesterRequest request) =>
        ModEntry.Instance.JesterApi!.HasCharacterFlag("dave_rigging") ? 2 : 0;

    public IJesterApi.StrategyCategory GetStrategyCategory() => IJesterApi.StrategyCategory.Full;

    private static List<RedBlack> GenerateDistribution(int cap, Rand rng)
    {
        var coloredCap = (cap + rng.NextInt() % cap) / 3;
        var redCap = coloredCap / 2;
        var blackCap = coloredCap - redCap;
        
        if (redCap != blackCap && rng.Next() < 0.5)
            (redCap, blackCap) = (blackCap, redCap);

        var distribution = Enumerable.Repeat(RedBlack.Blank, cap - coloredCap)
            .Concat(Enumerable.Repeat(RedBlack.Red, redCap))
            .Concat(Enumerable.Repeat(RedBlack.Black, blackCap))
            .ToList();
        ModEntry.Instance.JesterApi!.GetJesterUtil().Shuffle(distribution, rng);
        return distribution;
    }

    private enum RedBlack
    {
        Blank,
        Red,
        Black
    }
}

internal class SetupEntry : IJesterApi.IEntry
{
    public required Guid Guid { get; init; }
    
    public IReadOnlySet<string> Tags => new HashSet<string>();

    public IEnumerable<CardAction> GetActions(State s, Combat c) =>
    [
        RandomChoiceActionFactory.MakeSetupAction(Guid)
    ];

    public int GetCost() => 0;

    public IEnumerable<(double, IJesterApi.IEntry)>
        GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir) => [];

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
    }
}

internal class RedBlackEntry : IJesterApi.IEntry
{
    public required IJesterApi.IEntry Inner { get; init; }
    public required Guid Guid { get; init; }
    public required bool IsRed { get; init; }

    public IReadOnlySet<string> Tags => Inner.Tags;

    public IEnumerable<CardAction> GetActions(State s, Combat c) =>
        Inner.GetActions(s, c)
            .Select(a => RandomChoiceActionFactory.MakeRedBlackAction(Guid, IsRed, a));

    public int GetCost() => Inner.GetCost() * 3 / 4;

    public IEnumerable<(double, IJesterApi.IEntry)>
        GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir) =>
        Inner.GetUpgradeOptions(request, upDir)
            .Select(oe =>
            {
                var (o, e) = oe;
                return (o, new RedBlackEntry{ Guid = Guid, IsRed = IsRed, Inner = e } as IJesterApi.IEntry);
            });

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        Inner.AfterSelection(request);
    }
}