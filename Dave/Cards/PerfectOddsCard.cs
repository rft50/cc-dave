namespace Dave.Cards;

// 0-cost, 1 rig of your choice, temporary
[CardMeta(dontOffer = true, rarity = Rarity.common)]
public class PerfectOddsCard : Card
{
    public static Spr red_sprite;
    public static Spr black_sprite;

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return new List<CardAction>
        {
            new AStatus { status = ModEntry.Instance.RedRigging.Status, targetPlayer = true, statusAmount = 1, mode = AStatusMode.Add, disabled = flipped },
            new ADummyAction(),
            new AStatus { status = ModEntry.Instance.BlackRigging.Status, targetPlayer = true, statusAmount = 1, mode = AStatusMode.Add, disabled = !flipped }
        };
    }

    public override CardData GetData(State state) => new()
    {
        cost = 0,
        art = flipped ? black_sprite : red_sprite,
        floppable = true,
        temporary = true,
        retain = true,
        exhaust = true,
        artTint = "ffffff"
    };
}