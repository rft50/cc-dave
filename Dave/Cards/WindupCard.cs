using Dave.Actions;

namespace Dave.Cards;

// 0-cost, -2 Overdrive, 1 Energy, Red to rig 2 Black
// A: -1 Overdrive
// B: Rig 3 instead
[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class WindupCard : Card
{
    public static Spr card_sprite;

    public override List<CardAction> GetActions(State s, Combat c)
    {
        var builtActions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
        {
            new AStatus { status = (Status)(ModManifest.black_rigging.Id ?? throw new Exception("missing status")), targetPlayer = true, statusAmount = upgrade == Upgrade.B ? 3 : 2, mode = AStatusMode.Add }
        });
        
        return new List<CardAction>
        {
            builtActions[0],
            new AStatus { status = Status.overdrive, statusAmount = upgrade == Upgrade.A ? -1 : -2, targetPlayer = true },
            new AEnergy { changeAmount = 1 },
            builtActions[1],
            new ADummyAction()
        };
    }

    public override CardData GetData(State state) => new()
    {
        cost = 0,
        art = card_sprite
    };
}