namespace Dave.Actions
{
    public class RandomMoveFoeAction : CardAction
    {
        internal static string glossary_item = "";

        public static Spr? spr;

        public int dist = 0;

        public override void Begin(G g, State s, Combat c)
        {
            c.QueueImmediate(new AMove { dir = dist, targetPlayer = false, isRandom = true });

            timer = 0;
        }
        
        public override Icon? GetIcon(State s)
        {
            if (spr == null)
                return null;
            return new Icon(spr.Value, dist, Colors.textMain);
        }

        public override List<Tooltip> GetTooltips(State s)
        {
            return new List<Tooltip> { new TTGlossary(glossary_item, dist) };
        }
    }
}