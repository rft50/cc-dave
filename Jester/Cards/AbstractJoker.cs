using Jester.Api;
using Jester.Generator;

namespace Jester.Cards;

public abstract class AbstractJoker : Card
{
    private IJesterResult? _cache;
    public int? Seed;
    public int Points;
    public int Energy;
    public string Category = null!;

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

            Points = minPts + rng.Next(0, ptsDelta);
        }

        _cache = JesterGenerator.GenerateCard(
            new JesterRequest
            {
                Seed = Seed.Value,
                FirstAction = Category,
                State = s,
                BasePoints = Points,
                CardData = new CardData
                {
                    cost = Energy
                }
            });
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        if (_cache == null)
            Setup(s);
        
        return _cache!.Entries.SelectMany(e => e.GetActions(s, c)).ToList();
    }

    public override CardData GetData(State state)
    {
        return _cache?.CardData ?? new CardData
        {
            cost = Energy
        };
    }
}