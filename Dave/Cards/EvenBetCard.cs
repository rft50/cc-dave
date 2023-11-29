namespace Dave.Cards
{
    // 1-cost, 2 Rigging of both colors, Exhaust
    // A: 0-cost
    // B: not Exhaust
    [CardMeta(rarity = Rarity.rare, upgradesTo = new [] { Upgrade.A, Upgrade.B })]
    public class EvenBetCard : Card
    {
        public static Spr card_sprite;

        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new List<CardAction>
            {
                new AStatus { status = (Status)(ModManifest.red_rigging.Id ?? throw new Exception("missing status")), targetPlayer = true, statusAmount = 2, mode = AStatusMode.Add },
                new AStatus { status = (Status)(ModManifest.black_rigging.Id ?? throw new Exception("missing status")), targetPlayer = true, statusAmount = 2, mode = AStatusMode.Add }
            };
        }

        public override CardData GetData(State state) => new()
        {
            cost = upgrade == Upgrade.A ? 0 : 1,
            art = card_sprite,
            exhaust = upgrade != Upgrade.B
        };
    }
}