using Dave.Actions;

namespace Dave.Cards;

// 1-cost, pow-move-pow-move-pow
// A: + move-pow
// b: moves and shots become stronger
[CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class WildBarrageCard : Card
{
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> list;
        var damage = this.GetDmg(s, 1);

        switch (upgrade)
        {
            default:
                list = new List<CardAction>
                {
                    new AAttack { damage = damage, fast = true },
                    new RandomMoveFoeAction { Dist = 1 },
                    new AAttack { damage = damage, fast = true },
                    new RandomMoveFoeAction { Dist = 1 },
                    new AAttack { damage = damage, fast = true }
                };
                break;
            case Upgrade.A:
                list = new List<CardAction>
                {
                    new AAttack { damage = damage, fast = true },
                    new RandomMoveFoeAction { Dist = 1 },
                    new AAttack { damage = damage, fast = true },
                    new RandomMoveFoeAction { Dist = 1 },
                    new AAttack { damage = damage, fast = true },
                    new RandomMoveFoeAction { Dist = 1 },
                    new AAttack { damage = damage, fast = true }
                };
                break;
            case Upgrade.B:
                list = new List<CardAction>
                {
                    new AAttack { damage = this.GetDmg(s, 1), fast = true },
                    new RandomMoveFoeAction { Dist = 2 },
                    new AAttack { damage = this.GetDmg(s, 2), fast = true },
                    new RandomMoveFoeAction { Dist = 3 },
                    new AAttack { damage = this.GetDmg(s, 3), fast = true }
                };
                break;
        }

        return list;
    }

    public override CardData GetData(State state) => new()
    {
        cost = 2
    };
}