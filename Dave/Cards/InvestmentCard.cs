namespace Dave.Cards;

// 1-cost, +1 Powerdrive, -4 Overdrive
// A: -3 Overdrive
// B: -2 Overdrive, but exhaust
[CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class InvestmentCard : Card
{
    public override List<CardAction> GetActions(State s, Combat c)
    {
        var cost = upgrade switch
        {
            Upgrade.A => -3,
            Upgrade.B => -2,
            _ => -4
        };

        return new List<CardAction>
        {
            new AStatus { status = Status.powerdrive, statusAmount = 1, targetPlayer = true },
            new AStatus { status = Status.overdrive, statusAmount = cost, targetPlayer = true }
        };
    }

    public override CardData GetData(State state) => new()
    {
        cost = 1,
        exhaust = upgrade == Upgrade.B
    };
}