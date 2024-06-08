namespace Dave.Actions
{
    public class BiasStatusAction : CardAction
    {
        internal static string RedGlossaryItem = null!;
        internal static string BlackGlossaryItem = null!;

        public static Spr? SprRed;
        public static Spr? SprBlack;

        public int Pow; // positive is red, negative is black

        public override void Begin(G g, State s, Combat c)
        {
            var ship = s.ship;
            var dir = Math.Sign(Pow);
            var total = Pow
                        + ship.Get(ModEntry.Instance.RedBias.Status)
                        - ship.Get(ModEntry.Instance.BlackBias.Status)
                        + dir * ship.Get(Status.boost);
            
            var actions = new List<CardAction>();
            
            if (ship.Get(Status.boost) > 0)
                actions.Add(new AStatus { status = Status.boost, targetPlayer = true, statusAmount = 0, mode = AStatusMode.Set });
            if (ship.Get(ModEntry.Instance.RedBias.Status) > 0 && total <= 0)
                actions.Add(new AStatus
                {
                    status = ModEntry.Instance.RedBias.Status,
                    targetPlayer = true,
                    statusAmount = 0,
                    mode = AStatusMode.Set
                });
            if (ship.Get(ModEntry.Instance.BlackBias.Status) > 0 && total >= 0)
                actions.Add(new AStatus
                {
                    status = ModEntry.Instance.BlackBias.Status,
                    targetPlayer = true,
                    statusAmount = 0,
                    mode = AStatusMode.Set
                });
            switch (total)
            {
                case > 0:
                    actions.Add(new AStatus
                    {
                        status = ModEntry.Instance.RedBias.Status,
                        targetPlayer = true,
                        statusAmount = total,
                        mode = AStatusMode.Set
                    });
                    break;
                case < 0:
                    actions.Add(new AStatus
                    {
                        status = ModEntry.Instance.BlackBias.Status,
                        targetPlayer = true,
                        statusAmount = -total,
                        mode = AStatusMode.Set
                    });
                    break;
            }
            
            c.QueueImmediate(actions);

            timer = 0;
        }

        public override Icon? GetIcon(State s)
        {
            return Pow switch
            {
                > 0 when SprRed != null => new Icon(SprRed.Value, Pow, Colors.textMain),
                < 0 when SprBlack != null => new Icon(SprBlack.Value, -Pow, Colors.textMain),
                _ => null
            };
        }
        
        public override List<Tooltip> GetTooltips(State s)
        {
            return new List<Tooltip>
            {
                Pow > 0
                ? new TTGlossary(RedGlossaryItem, Pow)
                : new TTGlossary(BlackGlossaryItem, -Pow)
            };
        }
    }
}