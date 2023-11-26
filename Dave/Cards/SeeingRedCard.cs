using Dave.Actions;

namespace Dave.Cards;

// 1-cost, Black to 2 temp shield, 2 shield hurt, 1 Overdrive, Red to 1 Overdrive
// A: shield and shield hurt are 1
// B: shield and shield hurt are 3, main Overdrive is 2
[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class SeeingRedCard : Card
{
    private static Spr card_sprite = Spr.cards_GoatDrone;

    public override List<CardAction> GetActions(State s, Combat c)
    {
        var shieldHurt = upgrade switch
        {
            Upgrade.A => 1,
            Upgrade.B => 3,
            _ => 2
        };

        var builtActions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
        {
            new AStatus { status = Status.overdrive, statusAmount = 1, targetPlayer = true }
        }, new List<CardAction>
        {
            new AStatus { status = Status.tempShield, targetPlayer = true, statusAmount = shieldHurt, mode = AStatusMode.Add }
        });

        return new List<CardAction>
        {
            builtActions[0],
            builtActions[2],
            new ShieldHurtAction { dmg = shieldHurt },
            new AStatus { status = Status.overdrive, statusAmount = upgrade == Upgrade.B ? 2 : 1, targetPlayer = true },
            builtActions[1],
            new ADummyAction()
        };
    }

    public override CardData GetData(State state) => new()
    {
        cost = 1,
        art = card_sprite
    };
}