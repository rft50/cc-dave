using Jester.Actions;

namespace Jester.Cards;

[CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class FinalAct : Card
{
    public override List<CardAction> GetActions(State s, Combat c) => new()
    {
        new FinalActAction
        {
            Upgrade = upgrade
        }
    };

    public override CardData GetData(State state) => new()
    {
        cost = 1,
        singleUse = true,
        retain = true,
        description = GetDescription()
    };

    private string GetDescription()
    {
        return upgrade switch
        {
            Upgrade.A => "Select a card in hand, give it single use and play it three times.",
            Upgrade.B => "Select a card in hand, give it single use and play it. This card returns next boss.",
            _ => "Select a card in hand, give it single use and play it."
        };
    }
}