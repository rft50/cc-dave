using Jester.Actions;

namespace Jester.Cards;

// 2-cost, set cost of all cards in hand to 0-3
// A: 1-cost
// B: retain
[CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class SmokeAndMirrors : Card
{
    public override List<CardAction> GetActions(State s, Combat c) => new()
    {
        new SmokeAndMirrorsAction()
    };

    public override CardData GetData(State state) => new()
    {
        cost = upgrade == Upgrade.A ? 1 : 2,
        retain = upgrade == Upgrade.B,
        description = "Set the costs of all cards in hand to 0-3. Lasts one play."
    };
}