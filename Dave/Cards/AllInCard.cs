using Dave.Actions;

namespace Dave.Cards
{
    // 3-cost, 3 shield hurt on Black, 10 damage on Red, exhaust
    // A: Damage goes first
    // B: Switch Red/Black parity
    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class AllInCard : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> actions;

            switch (upgrade)
            {
                default:
                    actions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                    {
                        new AAttack { damage = GetDmg(s, 10) }
                    }, new List<CardAction>
                    {
                        new ShieldHurtAction { dmg = 3 }
                    });
                    actions = new List<CardAction>
                    {
                        actions[0],
                        actions[2],
                        actions[1]
                    };
                    break;
                case Upgrade.A:
                    actions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                    {
                        new AAttack { damage = GetDmg(s, 10) }
                    }, new List<CardAction>
                    {
                        new ShieldHurtAction { dmg = 3 }
                    });
                    break;
                case Upgrade.B:
                    actions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                    {
                        new ShieldHurtAction { dmg = 3 }
                    }, new List<CardAction>
                    {
                        new AAttack { damage = GetDmg(s, 10) }
                    });
                    break;
            }

            return actions;
        }

        public override CardData GetData(State state) => new()
        {
            cost = 3,
            exhaust = true
        };
    }
}