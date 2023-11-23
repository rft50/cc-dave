namespace Dave.Actions
{
    public class RandomChoiceActionFactory
    {
        public static Color RedColor = new("ae2334");
        public static Color BlackColor = new("2e222f");
        private static readonly Random Random = new();
        public static string glossary_item = "";
        
        // 0th element is a setup action that should be the first action
        // after that it's all the blues, then all the oranges
        // you probably want to put a dummy action at the end to fix the render issue of the setup action
        public static List<CardAction> BuildActions(List<CardAction>? red, List<CardAction>? black = null)
        {
            var data = new RandomChoiceActionData();

            var actions = new List<CardAction>
            {
                new RandomChoiceSetupAction { data = data }
            };
            
            if (red != null)
                actions.AddRange(red.Select(t => new RedAction { action = t, data = data }));
            
            if (black != null)
                actions.AddRange(black.Select(t => new BlackAction { action = t, data = data }));

            return actions;
        }

        internal class RandomChoiceActionData
        {
            public bool isRed;
            public bool isBlack;
        }

        internal class RandomChoiceSetupAction : CardAction
        {
            public RandomChoiceActionData data;

            public override void Begin(G g, State s, Combat c)
            {
                var blueOdds = 0.5;
                blueOdds += s.ship.Get((Status)(ModManifest.red_bias?.Id ?? throw new Exception()));
                blueOdds -= s.ship.Get((Status)(ModManifest.black_bias?.Id ?? throw new Exception()));

                if (s.ship.Get((Status)(ModManifest.red_rigging?.Id ?? throw new Exception())) > 0)
                {
                    data.isRed = true;
                    c.QueueImmediate(new AStatus { status = (Status)ModManifest.red_rigging.Id, targetPlayer = true, statusAmount = -1, mode = AStatusMode.Add });
                }

                if (s.ship.Get((Status)(ModManifest.black_rigging?.Id ?? throw new Exception())) > 0)
                {
                    data.isBlack = true;
                    c.QueueImmediate(new AStatus { status = (Status)ModManifest.black_rigging.Id, targetPlayer = true, statusAmount = -1, mode = AStatusMode.Add });
                }

                if (data is { isRed: false, isBlack: false })
                {
                    data.isRed = Random.NextDouble() < blueOdds;
                    data.isBlack = !data.isRed;
                }

                timer = 0;
            }

            public override Icon? GetIcon(State s)
            {
                return null;
            }

            public override List<Tooltip> GetTooltips(State s)
            {
                return new List<Tooltip> { new TTGlossary(glossary_item) };
            }
        }
        
        internal class RedAction : CardAction
        {
            public RandomChoiceActionData data;
            public CardAction action;

            public override void Begin(G g, State s, Combat c)
            {
                if (data.isRed)
                {
                    c.QueueImmediate(action);
                }

                timer = 0;
            }

            public override Icon? GetIcon(State s)
            {
                var icon = action.GetIcon(s);

                if (!icon.HasValue) return icon;
                var i = icon.Value;
                i.color = RedColor;

                return i;
            }

            public override List<Tooltip> GetTooltips(State s)
            {
                return action.GetTooltips(s);
            }
        }

        internal class BlackAction : CardAction
        {
            public RandomChoiceActionData data;
            public CardAction action;

            public override void Begin(G g, State s, Combat c)
            {
                if (data.isBlack)
                {
                    c.QueueImmediate(action);
                }

                timer = 0;
            }

            public override Icon? GetIcon(State s)
            {
                var icon = action.GetIcon(s);
                
                if (!icon.HasValue) return icon;
                var i = icon.Value;
                i.color = BlackColor;

                return i;
            }

            public override List<Tooltip> GetTooltips(State s)
            {
                return action.GetTooltips(s);
            }
        }
    }
}