using Jester.Generator;

namespace Jester.Cards;

[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class Joker : Card
{
    private JesterResult? _cache;
    public int Seed; // make public for serialization
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        if (Seed == 0)
            Seed = Random.Shared.Next();
        
        var rng = new Random(Seed);
        
        _cache ??= JesterGenerator.GenerateCard(
            new JesterRequest
            {
                Seed = Seed,
                FirstAction = "attack",
                State = s,
                BasePoints = 20 + rng.Next(0, 8),
                CardData = new CardData
                {
                    cost = 1
                }
            });

        return _cache.Entries.SelectMany(e => e.GetActions(s, c)).ToList();
    }

    public override CardData GetData(State state)
    {
        return _cache?.CardData ?? new CardData
        {
            cost = 8
        };
    }

    public override void OnFlip(G g)
    {
        Seed++;
        _cache = null;
    }
}