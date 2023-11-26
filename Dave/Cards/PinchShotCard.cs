using Dave.Actions;

namespace Dave.Cards;

// 1-cost, Red to 3 damage shot, Black to 2 temp shield, 1 shield hurt
// A: 5 damage, 2 shield hurt
// B: 3 temp shield
[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class PinchShotCard : Card
{
    private static Spr card_sprite = Spr.cards_GoatDrone;

    public override List<CardAction> GetActions(State s, Combat c)
    {
        var builtActions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
        {
            new AAttack { damage = this.GetDmg(s, upgrade == Upgrade.A ? 5 : 3) }
        }, new List<CardAction>
        {
            new AStatus { status = Status.tempShield, targetPlayer = true, statusAmount = upgrade == Upgrade.B ? 3 : 2, mode = AStatusMode.Add }
        });

        return new List<CardAction>
        {
            builtActions[0],
            builtActions[1],
            builtActions[2],
            new ShieldHurtAction { dmg = upgrade == Upgrade.A ? 2 : 1 },
            new ADummyAction()
        };
    }

    public override CardData GetData(State state) => new()
    {
        cost = 1,
        art = card_sprite
    };
}