using Nickel;

namespace Dave.Actions;

public class ShieldHurtAction : AHurt
{
    internal static Spr? Spr;

    public override void Begin(G g, State s, Combat c)
    {
        hurtShieldsFirst = true;
        targetPlayer = true;

        base.Begin(g, s, c);
    }

    public override Icon? GetIcon(State s)
    {
        if (Spr == null)
            return null;
        return new Icon(Spr.Value, hurtAmount, Colors.textMain);
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
                    hurtAmount
                ),
                TitleColor = Colors.action,
                Icon = Spr
            }
        ];
    }
}