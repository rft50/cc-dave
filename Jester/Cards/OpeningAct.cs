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
                "Select a card non-Opening Acted card in hand. It will be played at the start of all future battles while in your deck. You will gain Opening Fatigue equal to its cost minus one.",
            _ =>
                "Select a card non-Opening Acted card in hand. It will be played at the start of all future battles while in your deck. You will gain Opening Fatigue equal to its cost."
        };
    }
}