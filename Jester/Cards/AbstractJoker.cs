﻿using Jester.Api;
using Jester.Generator;
using Shockah.Shared;

namespace Jester.Cards;

public abstract class AbstractJoker : Card
{
    private IJesterApi.IJesterResult? _cache;
    private IJesterApi.IJesterResult? _cacheA;
    private IJesterApi.IJesterResult? _cacheB;
    public int? Seed;
    public int Points;
    public int? Energy;
    public string Category = null!;
    public bool SingleUse = false;

    private void Setup(State s)
    {
        if (Seed == null)
        {
            var metadata = GetMeta();
            var rarity = metadata.rarity;
            Seed = s.rngActions.NextInt();
            var rng  = new Rand((uint)Seed);
            Energy ??= rng.NextInt() % (rarity == Rarity.common ? 4 : 5);

            int ptsBase;
            int ptsStep;

            switch (rarity)
            {
                case Rarity.common:
                    ptsBase = 5;
                    ptsStep = 15;
                    break;
                case Rarity.uncommon:
                    ptsBase = 10;
                    ptsStep = 20;
                    break;
                case Rarity.rare:
                    ptsBase = 10;
                    ptsStep = 25;
                    break;
                default:
                    ptsBase = 10;
                    ptsStep = 10;
                    break;
            }

            if (Energy == 0)
                Points = ptsBase + 5;
            else
                Points = ptsBase + ptsStep * Energy.Value;

            if (SingleUse)
                Points += 25;
        }

        var request = new JesterRequest
        {
            Seed = Seed.Value,
            FirstAction = Category,
            State = s,
            BasePoints = Points,
            CardData = new CardData
            {
                cost = Energy!.Value,
                singleUse = SingleUse
            },
            CardMeta = GetMeta()
        };
        _cache = JesterGenerator.GenerateCard(request);
        _cacheA = JesterGenerator.UpgradeResultA(request, _cache);
        _cacheB = JesterGenerator.UpgradeResultB(request, _cache);
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        if (_cache == null)
            Setup(s);
        
        return GetCache()!.Entries.SelectMany(e => e.GetActions(s, c)).ToList();
    }

    public override CardData GetData(State state)
    {
        var g = GExt.Instance!;
        if (g.state.route is NewRunOptions or CardBrowse { browseSource: CardBrowse.Source.Codex })
            return new CardData
            {
                cost = 1,
                description = $"A random {GetMeta().rarity} {Category} card."
            };
        
        return GetCache()?.CardData ?? new CardData
        {
            cost = Energy ?? 0
        };
    }

    private IJesterApi.IJesterResult? GetCache()
    {
        switch (upgrade)
        {
            case Upgrade.B:
                return _cacheB;
            case Upgrade.A:
                return _cacheA;
            default:
                return _cache;
        }
    }

    public void ClearCache()
    {
        _cache = null;
        _cacheA = null;
        _cacheB = null;
    }
}