using Dave.Actions;

namespace Dave.Cards
{
    // 0-cost, -1 on red, +1 on black
    // A: -1 on red, +2 on black
    // B: -2 on red, +3 on black
    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class RaiseCard : Card
    {
        private static Spr card_sprite = Spr.cards_GoatDrone;
        
        public override List<CardAction> GetActions(State s, Combat c)
        {
            var actions = new List<CardAction>();

            switch (upgrade)
            {
                default:
                    actions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                    {
                        new AStatus { status = Status.overdrive, statusAmount = -1, targetPlayer = true }
                    }, new List<CardAction>
                    {
                        new AStatus { status = Status.overdrive, statusAmount = 1, targetPlayer = true }
                    });
                    actions.Add(new ADummyAction());
                    break;
                case Upgrade.A:
                    actions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                    {
                        new AStatus { status = Status.overdrive, statusAmount = -1, targetPlayer = true }
                    }, new List<CardAction>
                    {
                        new AStatus { status = Status.overdrive, statusAmount = 2, targetPlayer = true }
                    });
                    actions.Add(new ADummyAction());
                    break;
                case Upgrade.B:
                    actions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                    {
                        new AStatus { status = Status.overdrive, statusAmount = -2, targetPlayer = true }
                    }, new List<CardAction>
                    {
                        new AStatus { status = Status.overdrive, statusAmount = 3, targetPlayer = true }
                    });
                    actions.Add(new ADummyAction());
                    break;
            }

            return actions;
        }

        public override CardData GetData(State state) => new()
        {
            cost = 0,
            art = card_sprite
        };
    }
}