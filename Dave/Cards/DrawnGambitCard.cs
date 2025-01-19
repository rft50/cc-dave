namespace Dave.Cards;

// 0-cost, draw to 10, discard down to old hand size, exhaust
// A: keep 2 extra cards
// B: 1-cost, don't exhaust
[CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class DrawnGambitCard : Card
{
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions;

        var currentHand = c.hand.Count;

        switch (upgrade)
        {
            default:
                actions = new List<CardAction>
                {
                    new ADrawCard { count = 10 - currentHand, omitFromTooltips = true },
                    new ADiscard { count = 10 - currentHand, omitFromTooltips = true }
                };
                break;
            case Upgrade.A:
                actions = new List<CardAction>
                {
                    new ADrawCard { count = 10 - currentHand, omitFromTooltips = true },
                    new ADiscard { count = 8 - currentHand, omitFromTooltips = true }
                };
                break;
        }

        return actions;
    }

    public override CardData GetData(State state) => new()
    {
        cost = upgrade == Upgrade.B ? 1 : 0,
        exhaust = upgrade != Upgrade.B,
        description = upgrade == Upgrade.A
        ? "Draw to 10 cards, then discard to your old hand size + 2."
        : "Draw to 10 cards, then discard to your old hand size."
    };
}