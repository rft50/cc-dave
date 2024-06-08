namespace Dave.Actions
{
    public class RandomMoveFoeAction : CardAction
    {
        internal static string glossary_item = "";

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
            return new List<Tooltip> { new TTGlossary(glossary_item, Dist) };
        }
    }
}