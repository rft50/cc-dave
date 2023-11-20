using Dave.Actions;

namespace Dave.Cards
{
    // 1-cost, shoot once, maybe shoot two more times
    // A: shoot twice, maybe shoot three more times
    // b: piercing once, either piercing twice or shoot once
    [CardMeta(deck = Deck.riggs, rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class WildShotCard : Card
    {
        private static Spr card_sprite = Spr.cards_GoatDrone;

        public override List<CardAction> GetActions(State s, Combat c)
        {
            var actions = new List<CardAction>();
            var damage = this.GetDmg(s, 1);

            switch (upgrade)
            {
                default:
                    actions.Add(new AAttack { damage = damage, fast = true});
                    actions.AddRange(RandomChoiceActionFactory.BuildActions(new List<CardAction>
                    {
                        new AAttack { damage = damage, fast = true },
                        new AAttack { damage = damage, fast = true }
                    }));
                    break;
                case Upgrade.A:
                    actions.Add(new AAttack { damage = damage, fast = true });
                    actions.Add(new AAttack { damage = damage, fast = true });
                    actions.AddRange(RandomChoiceActionFactory.BuildActions(new List<CardAction>
                    {
                        new AAttack { damage = damage, fast = true },
                        new AAttack { damage = damage, fast = true },
                        new AAttack { damage = damage, fast = true }
                    }));
                    break;
                case Upgrade.B:
                    actions.Add(new AAttack { damage = damage, fast = true, piercing = true });
                    actions.AddRange(RandomChoiceActionFactory.BuildActions(new List<CardAction>
                    {
                        new AAttack { damage = damage, fast = true, piercing = true },
                        new AAttack { damage = damage, fast = true, piercing = true }
                    }, new List<CardAction>
                    {
                        new AAttack { damage = damage, fast = true, stunEnemy = true }
                    }));
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