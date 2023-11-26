using Dave.Actions;

namespace Dave.Cards
{
    // 1-cost, 1 rig of your choice
    // A: 2 rigs
    // B: 3 rigs, blue/orange, opposite of roll
    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new [] { Upgrade.A, Upgrade.B })]
    public class RiggingCard : Card
    {
        private static Spr card_sprite = Spr.cards_GoatDrone;

        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> list;

            switch (upgrade)
            {
                default:
                    list = new List<CardAction>
                    {
                        new AStatus { status = (Status)(ModManifest.red_rigging.Id ?? throw new Exception("missing status")), targetPlayer = true, statusAmount = 2, mode = AStatusMode.Add, disabled = flipped },
                        new ADummyAction(),
                        new AStatus { status = (Status)(ModManifest.black_rigging.Id ?? throw new Exception("missing status")), targetPlayer = true, statusAmount = 2, mode = AStatusMode.Add, disabled = !flipped }
                    };
                    break;
                case Upgrade.A:
                    list = new List<CardAction>
                    {
                        new AStatus { status = (Status)(ModManifest.red_rigging.Id ?? throw new Exception("missing status")), targetPlayer = true, statusAmount = 3, mode = AStatusMode.Add, disabled = flipped },
                        new ADummyAction(),
                        new AStatus { status = (Status)(ModManifest.black_rigging.Id ?? throw new Exception("missing status")), targetPlayer = true, statusAmount = 3, mode = AStatusMode.Add, disabled = !flipped }
                    };
                    break;
                case Upgrade.B:
                    list = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                    {
                        new AStatus { status = (Status)(ModManifest.black_rigging.Id ?? throw new Exception("missing status")), targetPlayer = true, statusAmount = 3, mode = AStatusMode.Add }
                    }, new List<CardAction>
                    {
                        new AStatus { status = (Status)(ModManifest.red_rigging.Id ?? throw new Exception("missing status")), targetPlayer = true, statusAmount = 3, mode = AStatusMode.Add },
                    });
                    list.Add(new ADummyAction());
                    break;
            }

            return list;
        }

        public override CardData GetData(State state) => new()
        {
            cost = 1,
            art = card_sprite,
            floppable = upgrade != Upgrade.B
        };
    }
}