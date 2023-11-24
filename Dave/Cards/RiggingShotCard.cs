using Dave.Actions;

namespace Dave.Cards
{
    // 1-cost, shoot once, consume Red to shoot once, rig Black
    // A: red shot is 2 damage
    // B: red has a second, 0-damage shot
    [CardMeta(rarity = Rarity.common, upgradesTo = new [] { Upgrade.A, Upgrade.B })]
    public class RiggingShotCard : Card
    {
        private static Spr card_sprite = Spr.cards_GoatDrone;

        public override List<CardAction> GetActions(State s, Combat c)
        {
            var actions = new List<CardAction>();
            List<CardAction> builtActions;

            switch (upgrade)
            {
                default:
                    builtActions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                    {
                        new AAttack { damage = this.GetDmg(s, 1), fast = true }
                    });
                    actions.Add(builtActions[0]);
                    actions.Add(new AAttack { damage = this.GetDmg(s, 1), fast = true });
                    actions.Add(builtActions[1]);
                    actions.Add(new AStatus
                    {
                        status = (Status)(ModManifest.black_rigging.Id ?? throw new Exception("missing status")),
                        targetPlayer = true,
                        statusAmount = 1,
                        mode = AStatusMode.Add
                    });
                    actions.Add(new ADummyAction());
                    break;
                case Upgrade.A:
                    builtActions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                    {
                        new AAttack { damage = this.GetDmg(s, 2), fast = true }
                    });
                    actions.Add(builtActions[0]);
                    actions.Add(new AAttack { damage = this.GetDmg(s, 1), fast = true });
                    actions.Add(builtActions[1]);
                    actions.Add(new AStatus
                    {
                        status = (Status)(ModManifest.black_rigging.Id ?? throw new Exception("missing status")),
                        targetPlayer = true,
                        statusAmount = 1,
                        mode = AStatusMode.Add
                    });
                    actions.Add(new ADummyAction());
                    break;
                case Upgrade.B:
                    builtActions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                    {
                        new AAttack { damage = this.GetDmg(s, 1), fast = true },
                        new AAttack { damage = this.GetDmg(s, 0), fast = true }
                    });
                    actions.Add(builtActions[0]);
                    actions.Add(new AAttack { damage = this.GetDmg(s, 1), fast = true });
                    actions.Add(builtActions[1]);
                    actions.Add(builtActions[2]);
                    actions.Add(new AStatus
                    {
                        status = (Status)(ModManifest.black_rigging.Id ?? throw new Exception("missing status")),
                        targetPlayer = true,
                        statusAmount = 1,
                        mode = AStatusMode.Add
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