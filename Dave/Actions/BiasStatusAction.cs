namespace Dave.Actions
{
    public class BiasStatusAction : CardAction
    {
        public static string blue_glossary_item = "";
        public static string orange_glossary_item = "";

        public static Spr? spr_red;
        public static Spr? spr_black;

        public int pow; // positive is red, negative is black

        public override void Begin(G g, State s, Combat c)
        {
            var ship = s.ship;
            var dir = Math.Sign(pow);
            var total = pow
                        + ship.Get((Status)(ModManifest.red_bias?.Id ?? throw new NullReferenceException()))
                        - ship.Get((Status)(ModManifest.black_bias?.Id ?? throw new NullReferenceException()))
                        + dir * ship.Get(Status.boost);
            
            var actions = new List<CardAction>();
            
            if (ship.Get(Status.boost) > 0)
                actions.Add(new AStatus { status = Status.boost, targetPlayer = true, statusAmount = 0, mode = AStatusMode.Set });
            if (ship.Get((Status)ModManifest.red_bias.Id) > 0 && total <= 0)
                actions.Add(new AStatus
                {
                    status = (Status)ModManifest.red_bias.Id,
                    targetPlayer = true,
                    statusAmount = 0,
                    mode = AStatusMode.Set
                });
            if (ship.Get((Status)ModManifest.black_bias.Id) > 0 && total >= 0)
                actions.Add(new AStatus
                {
                    status = (Status)ModManifest.black_bias.Id,
                    targetPlayer = true,
                    statusAmount = 0,
                    mode = AStatusMode.Set
                });
            switch (total)
            {
                case > 0:
                    actions.Add(new AStatus
                    {
                        status = (Status)ModManifest.red_bias.Id,
                        targetPlayer = true,
                        statusAmount = total,
                        mode = AStatusMode.Set
                    });
                    break;
                case < 0:
                    actions.Add(new AStatus
                    {
                        status = (Status)ModManifest.black_bias.Id,
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
            return pow switch
            {
                > 0 when spr_red != null => new Icon(spr_red.Value, pow, Colors.textMain),
                < 0 when spr_black != null => new Icon(spr_black.Value, -pow, Colors.textMain),
                _ => null
            };
        }
        
        public override List<Tooltip> GetTooltips(State s)
        {
            return new List<Tooltip>
            {
                pow > 0
                ? new TTGlossary(blue_glossary_item, pow)
                : new TTGlossary(orange_glossary_item, -pow)
            };
        }
    }
}