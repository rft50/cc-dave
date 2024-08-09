using Dave.Actions;

namespace Dave.Cards;

// 1-cost, 2 temp shield, Red to 1 shield hurt, Black to 2 temp shield
// A: 3 temp shield
// B: Black to 2 shield & 1 temp shield
[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class WildWallCard : Card
{
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions;
        List<CardAction> builtActions;

        switch (upgrade)
        {
            default:
                builtActions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                {
                    new ShieldHurtAction { dmg = 1 }
                }, new List<CardAction>
                {
                    new AStatus { status = Status.tempShield, targetPlayer = true, statusAmount = 2, mode = AStatusMode.Add }
                });
                actions = new List<CardAction>
                {
                    builtActions[0],
                    new AStatus { status = Status.tempShield, targetPlayer = true, statusAmount = 2, mode = AStatusMode.Add },
                    builtActions[1],
                    builtActions[2]
                };
                break;
            case Upgrade.A:
                builtActions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                {
                    new ShieldHurtAction { dmg = 1 }
                }, new List<CardAction>
                {
                    new AStatus { status = Status.tempShield, targetPlayer = true, statusAmount = 2, mode = AStatusMode.Add }
                });
                actions = new List<CardAction>
                {
                    builtActions[0],
                    new AStatus { status = Status.tempShield, targetPlayer = true, statusAmount = 3, mode = AStatusMode.Add },
                    builtActions[1],
                    builtActions[2]
                };
                break;
            case Upgrade.B:
                builtActions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                {
                    new ShieldHurtAction { dmg = 1 }
                }, new List<CardAction>
                {
                    new AStatus { status = Status.shield, targetPlayer = true, statusAmount = 2, mode = AStatusMode.Add },
                    new AStatus { status = Status.tempShield, targetPlayer = true, statusAmount = 1, mode = AStatusMode.Add }
                });
                actions = new List<CardAction>
                {
                    builtActions[0],
                    new AStatus { status = Status.tempShield, targetPlayer = true, statusAmount = 2, mode = AStatusMode.Add },
                    builtActions[1],
                    builtActions[2],
                    builtActions[3]
                };
                break;
        }

        return actions;
    }

    public override CardData GetData(State state) => new()
    {
        cost = 1
    };
}