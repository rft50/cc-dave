using Dave.Actions;

namespace Dave.Cards;

// 1-cost, 2s rig of your choice
// A: 3 rigs
// B: 3 rigs, red/black, opposite of roll
[CardMeta(rarity = Rarity.uncommon, upgradesTo = new [] { Upgrade.A, Upgrade.B })]
public class RiggingCard : Card
{
    public static Spr red_sprite;
    public static Spr black_sprite;

    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> list;

        switch (upgrade)
        {
            default:
                list = new List<CardAction>
                {
                    new AStatus { status = ModEntry.Instance.RedRigging.Status, targetPlayer = true, statusAmount = 2, mode = AStatusMode.Add, disabled = flipped },
                    new ADummyAction(),
                    new AStatus { status = ModEntry.Instance.BlackRigging.Status, targetPlayer = true, statusAmount = 2, mode = AStatusMode.Add, disabled = !flipped }
                };
                break;
            case Upgrade.A:
                list = new List<CardAction>
                {
                    new AStatus { status = ModEntry.Instance.RedRigging.Status, targetPlayer = true, statusAmount = 3, mode = AStatusMode.Add, disabled = flipped },
                    new ADummyAction(),
                    new AStatus { status = ModEntry.Instance.BlackRigging.Status, targetPlayer = true, statusAmount = 3, mode = AStatusMode.Add, disabled = !flipped }
                };
                break;
            case Upgrade.B:
                list = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                {
                    new AStatus { status = ModEntry.Instance.BlackRigging.Status, targetPlayer = true, statusAmount = 3, mode = AStatusMode.Add }
                }, new List<CardAction>
                {
                    new AStatus { status = ModEntry.Instance.RedRigging.Status, targetPlayer = true, statusAmount = 3, mode = AStatusMode.Add },
                });
                break;
        }

        return list;
    }

    public override CardData GetData(State state) => new()
    {
        cost = 1,
        art = upgrade == Upgrade.B ? null : (flipped ? black_sprite : red_sprite),
        floppable = upgrade != Upgrade.B,
        artTint = upgrade != Upgrade.B ? "ffffff" : null
    };
}