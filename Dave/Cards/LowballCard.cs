namespace Dave.Cards;

// 2-cost, both parties get -2 Overdrive, exhaust
// A: both get -3
// B: you only get -1
[CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class LowballCard : Card
{
    public override List<CardAction> GetActions(State s, Combat c)
    {
        var outgoing = upgrade switch
        {
            Upgrade.A => -3,
            _ => -2
        };

        var incoming = upgrade switch
        {
            Upgrade.A => -3,
            Upgrade.B => -1,
            _ => -2
        };

        return new List<CardAction>
        {
            new AStatus { status = Status.overdrive, statusAmount = outgoing, targetPlayer = false },
            new AStatus { status = Status.overdrive, statusAmount = incoming, targetPlayer = true }
        };
    }

    public override CardData GetData(State state) => new()
    {
        cost = 2,
        exhaust = true
    };
}