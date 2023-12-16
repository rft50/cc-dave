using Jester.Actions;

namespace Jester.Cards;

// 1-cost, add 1 of 3 single use, generated, temp Jester cards to your hand
// A: 0-cost
// B: 5 options
[CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class MadCackle : Card
{
    public override List<CardAction> GetActions(State s, Combat c) => new()
    {
        new MadCackleAction
        {
            OfferCount = upgrade == Upgrade.B ? 5 : 3
        }
    };

    public override CardData GetData(State state) => new CardData
    {
        cost = upgrade == Upgrade.A ? 0 : 1,
        description = upgrade == Upgrade.B
            ? "Add 1 of 5 <c=cardtrait>single use, generated, temp</c> <c=keyword>Jester</c> cards to your hand."
            : "Add 1 of 3 <c=cardtrait>single use, generated, temp</c> <c=keyword>Jester</c> cards to your hand."
    };
}