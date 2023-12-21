using Jester.Actions;

namespace Jester.Cards;

// 1-cost, Pick a card you've played this turn. Perform its actions again. Exhaust
// A: Retain
// B: Then put it back in your hand
[CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class Encore : Card
{
    public override List<CardAction> GetActions(State s, Combat c) => new()
    {
        new EncoreAction
        {
            ReturnCard = upgrade == Upgrade.B
        }
    };

    public override CardData GetData(State state) => new()
    {
        cost = 1,
        exhaust = true,
        retain = upgrade == Upgrade.A,
        description = upgrade == Upgrade.B
            ? "Pick a card you've played this turn. Play it for free. Then put it back into your hand."
            : "Pick a card you've played this turn. Play it for free."
    };
}