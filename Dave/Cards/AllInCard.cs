using Dave.Actions;

namespace Dave.Cards
{
    // 1-cost, 7 damage on Red, 2 bruise on Black
    // A: 7 damage on Red, 1 bruise on Black
    // B: 10 damage on Red, 3 bruise on Black
    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class AllInCard : Card
    {
        private static Spr card_sprite = Spr.cards_GoatDrone;
        
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> actions;

            switch (upgrade)
            {
                default:
                    actions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                    {
                        new AAttack { damage = this.GetDmg(s, 7) }
                    }, new List<CardAction>
                    {
                        new AHurt { hurtAmount = 2, targetPlayer = true }
                    });
                    actions.Add(new ADummyAction());
                    break;
                case Upgrade.A:
                    actions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                    {
                        new AAttack { damage = this.GetDmg(s, 7) }
                    }, new List<CardAction>
                    {
                        new AHurt { hurtAmount = 1, targetPlayer = true }
                    });
                    actions.Add(new ADummyAction());
                    break;
                case Upgrade.B:
                    actions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                    {
                        new AAttack { damage = this.GetDmg(s, 10) }
                    }, new List<CardAction>
                    {
                        new AHurt { hurtAmount = 3, targetPlayer = true }
                    });
                    actions.Add(new ADummyAction());
                    break;
            }

            return actions;
        }

        public override CardData GetData(State state) => new()
        {
            cost = 1,
            art = card_sprite
        };
    }
}