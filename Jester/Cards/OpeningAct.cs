using Jester.Actions;

namespace Jester.Cards;

[CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class OpeningAct : Card
{
    public override List<CardAction> GetActions(State s, Combat c) => new()
    {
        new OpeningActAction
        {
            Discounted = upgrade == Upgrade.B
        }
    };

    public override CardData GetData(State state) => new()
    {
        cost = upgrade == Upgrade.A ? 3 : 4,
        singleUse = true,
        description = GetDescription()
    };

    private string GetDescription()
    {
        return upgrade switch
        {
            Upgrade.B =>
                "Grant Opening Act to a non-temp card in hand without that trait. Gain one less Opening Fatigue for it.",
            _ =>
                "Grant Opening Act to a non-temp card in hand without that trait."
        };
    }
}