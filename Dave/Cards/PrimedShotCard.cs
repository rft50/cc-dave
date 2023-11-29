using Dave.Actions;

namespace Dave.Cards;

// 1-cost, 1 damage shot, Black to rig 2 Red
// A: 2 damage
// B: Rig 3 instead
[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class PrimedShotCard : Card
{
    public static Spr card_sprite;

    public override List<CardAction> GetActions(State s, Combat c)
    {
        var builtActions = RandomChoiceActionFactory.BuildActions(null, new List<CardAction>
        {
            new AStatus { status = (Status)(ModManifest.red_rigging.Id ?? throw new Exception("missing status")), targetPlayer = true, statusAmount = upgrade == Upgrade.B ? 3 : 2, mode = AStatusMode.Add }
        });
        
        return new List<CardAction>
        {
            builtActions[0],
            new AAttack { damage = this.GetDmg(s, upgrade == Upgrade.A ? 2 : 1) },
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