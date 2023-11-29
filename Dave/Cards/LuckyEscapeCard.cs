using Dave.Actions;

namespace Dave.Cards
{
    // 1-cost, random move 2, consume Black for Evade, rig Red
    // A: random move 3
    // B: gain 2 Evade
    [CardMeta(rarity = Rarity.common, upgradesTo = new [] { Upgrade.A, Upgrade.B })]
    public class LuckyEscapeCard : Card
    {
        public static Spr card_sprite;
        
        public override List<CardAction> GetActions(State s, Combat c)
        {
            var builtActions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
            {
                new AStatus { status = (Status)(ModManifest.red_rigging.Id ?? throw new Exception("missing status")), targetPlayer = true, statusAmount = 1, mode = AStatusMode.Add },
            }, new List<CardAction>
            {
                new AStatus { status = Status.evade, targetPlayer = true, statusAmount = upgrade == Upgrade.B ? 2 : 1, mode = AStatusMode.Add }
            });

            return new List<CardAction>
            {
                builtActions[0],
                new RandomMoveFoeAction { dist = upgrade == Upgrade.A ? 3 : 2 },
                builtActions[2],
                builtActions[1],
                new ADummyAction()
            };
        }

        public override CardData GetData(State state) => new()
        {
            cost = 1,
            art = card_sprite
        };
    }
}