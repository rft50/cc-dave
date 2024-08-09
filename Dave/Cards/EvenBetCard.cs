namespace Dave.Cards
{
    // 1-cost, 2 Rigging of both colors, Exhaust
    // A: 0-cost
    // B: not Exhaust
    [CardMeta(rarity = Rarity.rare, upgradesTo = new [] { Upgrade.A, Upgrade.B })]
    public class EvenBetCard : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new List<CardAction>
            {
                new AStatus { status = ModEntry.Instance.RedRigging.Status, targetPlayer = true, statusAmount = 2, mode = AStatusMode.Add },
                new AStatus { status = ModEntry.Instance.BlackRigging.Status, targetPlayer = true, statusAmount = 2, mode = AStatusMode.Add }
            };
        }

        public override CardData GetData(State state) => new()
        {
            cost = upgrade == Upgrade.A ? 0 : 1,
            exhaust = upgrade != Upgrade.B
        };
    }
}