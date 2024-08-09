using Nickel;

namespace Dave.Actions;

public class ShieldHurtAction : CardAction
{
    internal static Spr? Spr;

    public int dmg = 0;

    public override void Begin(G g, State s, Combat c)
    {
        c.QueueImmediate(new AHurt { hurtAmount = dmg, hurtShieldsFirst = true, targetPlayer = true });

        timer = 0;
    }

    public override Icon? GetIcon(State s)
    {
        if (Spr == null)
            return null;
        return new Icon(Spr.Value, dmg, Colors.textMain);
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        return
        [
            new GlossaryTooltip("Dave::action::ShieldHurt")
            {
                Title = ModEntry.Instance.Localizations.Localize(["action", "ShieldHurt", "name"]),
                Description = string.Format(
                    ModEntry.Instance.Localizations.Localize(["action", "ShieldHurt", "description"]),
                    dmg
                ),
                TitleColor = Colors.action,
                Icon = Spr
            }
        ];
    }
}