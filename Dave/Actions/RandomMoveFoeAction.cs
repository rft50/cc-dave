using Nickel;

namespace Dave.Actions
{
    public class RandomMoveFoeAction : CardAction
    {
        internal static Spr? Spr;

        public int Dist = 0;

        public override void Begin(G g, State s, Combat c)
        {
            c.QueueImmediate(new AMove { dir = Dist, targetPlayer = false, isRandom = true });

            timer = 0;
        }
        
        public override Icon? GetIcon(State s)
        {
            if (Spr == null)
                return null;
            return new Icon(Spr.Value, Dist, Colors.textMain);
        }

        public override List<Tooltip> GetTooltips(State s)
        {
            return
            [
                new GlossaryTooltip("Dave::action::RandomMoveFoe")
                {
                    Title = ModEntry.Instance.Localizations.Localize(["action", "RandomFoeMove", "name"]),
                    Description = string.Format(
                        ModEntry.Instance.Localizations.Localize(["action", "RandomFoeMove", "description"]),
                        Dist
                    ),
                    TitleColor = Colors.action,
                    Icon = Spr
                }
            ];
        }
    }
}