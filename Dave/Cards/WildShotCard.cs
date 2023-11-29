using Dave.Actions;

namespace Dave.Cards
{
    // 1-cost, shoot once, maybe shoot two more times
    // A: shoot twice, maybe shoot three more times
    // B: piercing once, either piercing twice or shoot once
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class WildShotCard : Card
    {
        public static Spr card_sprite;

        public override List<CardAction> GetActions(State s, Combat c)
        {
            var actions = new List<CardAction>();
            List<CardAction> builtActions;
            var damage = this.GetDmg(s, 1);

            switch (upgrade)
            {
                default:
                    builtActions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                    {
                        new AAttack { damage = damage, fast = true },
                        new AAttack { damage = damage, fast = true }
                    });
                    actions.Add(builtActions[0]);
                    actions.Add(new AAttack { damage = damage, fast = true});
                    actions.Add(builtActions[1]);
                    actions.Add(builtActions[2]);
                    actions.Add(new ADummyAction());
                    break;
                case Upgrade.A:
                    builtActions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                    {
                        new AAttack { damage = damage, fast = true },
                        new AAttack { damage = damage, fast = true },
                        new AAttack { damage = damage, fast = true }
                    });
                    actions.Add(builtActions[0]);
                    actions.Add(new AAttack { damage = damage, fast = true });
                    actions.Add(new AAttack { damage = damage, fast = true });
                    actions.Add(builtActions[1]);
                    actions.Add(builtActions[2]);
                    actions.Add(builtActions[3]);
                    actions.Add(new ADummyAction());
                    break;
                case Upgrade.B:
                    builtActions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                    {
                        new AAttack { damage = damage, fast = true, piercing = true },
                        new AAttack { damage = damage, fast = true, piercing = true }
                    }, new List<CardAction>
                    {
                        new AAttack { damage = damage, fast = true, stunEnemy = true }
                    });
                    actions.Add(builtActions[0]);
                    actions.Add(new AAttack { damage = damage, fast = true, piercing = true });
                    actions.Add(builtActions[1]);
                    actions.Add(builtActions[2]);
                    actions.Add(builtActions[3]);
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