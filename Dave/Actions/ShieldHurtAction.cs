namespace Dave.Actions;

public class ShieldHurtAction : CardAction
{
    internal static string glossary_item = "";

    public static Spr? spr;

    public int dmg = 0;

    public override void Begin(G g, State s, Combat c)
    {
        c.QueueImmediate(new AHurt { hurtAmount = dmg, hurtShieldsFirst = true, targetPlayer = true });

        timer = 0;
    }

    public override Icon? GetIcon(State s)
    {
        if (spr == null)
            return null;
        return new Icon(spr.Value, dmg, Colors.textMain);
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        return new List<Tooltip> { new TTGlossary(glossary_item, dmg) };
    }
}