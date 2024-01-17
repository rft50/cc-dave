using HarmonyLib;
using Jester.Api;
using Jester.Generator;

namespace Jester.Cards;

public abstract class AbstractJoker : Card
{
    private IJesterResult? _cache;
    private IJesterResult? _cacheA;
    private IJesterResult? _cacheB;
    public int? Seed;
    public int Points;
    public int Energy;
    public string Category = null!;
    public bool SingleUse = false;

    private void Setup(State s)
    {
        if (Seed == null)
        {
            Seed = Random.Shared.Next();
            var rng  = new Random(Seed.Value);

            var minPts = 20;
            var ptsDelta = 8;

            if (Energy == 0)
            {
                minPts /= 2;
                ptsDelta /= 2;
            }
            else
            {
                minPts *= Energy;
                ptsDelta *= Energy;
            }

            Points = minPts + rng.Next(0, ptsDelta) + (SingleUse ? 30 : 0);
        }

        var request = new JesterRequest
        {
            Seed = Seed.Value,
            FirstAction = Category,
            State = s,
            BasePoints = Points,
            CardData = new CardData
            {
                cost = Energy,
                singleUse = SingleUse
            }
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
        return GetCache()?.CardData ?? new CardData
        {
            cost = Energy
        };
    }

    private IJesterResult? GetCache()
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
}