using Dave.Actions;

namespace Dave.Cards;

// 1-cost, Red for 1 temp shield, Black for 1 Evade
// A: 2 temp shield
// B: 2 Evade
[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class FoldCard : Card
{
    public override List<CardAction> GetActions(State s, Combat c)
    {
        var actions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
        {
            new AStatus { status = Status.tempShield, targetPlayer = true, statusAmount = upgrade == Upgrade.A ? 2 : 1, mode = AStatusMode.Add }
        }, new List<CardAction>
        {
            new AStatus { status = Status.evade, targetPlayer = true, statusAmount = upgrade == Upgrade.B ? 2 : 1 }
        });
        
        return actions;
    }

    public override CardData GetData(State state) => new()
    {
        cost = 1
    };
}