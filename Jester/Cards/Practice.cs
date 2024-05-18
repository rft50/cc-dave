using Jester.Actions;

namespace Jester.Cards;

// 1-cost, discount a random card in hand
// A: two cards
// B: pick the card
[CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class Practice : Card
{
    public override List<CardAction> GetActions(State s, Combat c)
    {
        return upgrade switch
        {
            Upgrade.A => new List<CardAction>
            {
                new PracticeAction
                {
                    Random = true
                },
                new PracticeAction
                {
                    Random = true
                }
            },
            Upgrade.B => new List<CardAction>
            {
                new PracticeAction
                {
                    Random = false
                }
            },
            _ => new List<CardAction>
            {
                new PracticeAction
                {
                    Random = true
                }
            }
        };
    }

    public override CardData GetData(State state) => new()
    {
        cost = 0,
        description = upgrade switch
        {
            Upgrade.A => "Discount two random cards in hand.",
            Upgrade.B => "Discount the rightmost card in hand.",
            _ => "Discount a random card in hand."
        }
    };
}