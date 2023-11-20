namespace Dave.Actions
{
    public class RandomChoiceActionFactory
    {
        public static Color BlueColor = new Color("4d9be6");
        public static Color OrangeColor = new Color("f57d4a");
        private static readonly Random Random = new Random();
        public static string glossary_item = "";
        
        public static List<CardAction> BuildActions(List<CardAction> blue, List<CardAction>? orange = null)
        {
            var data = new RandomChoiceActionData();
            var actions = blue.Select((t, i) => new BlueAction { isFirst = i == 0, action = t, data = data }).Cast<CardAction>().ToList();
            
            if (orange != null)
                actions.AddRange(orange.Select(t => new OrangeAction { action = t, data = data }));

            return actions;
        }

        internal class RandomChoiceActionData
        {
            public bool isBlue;
            public bool isOrange;
        }
        
        internal class BlueAction : CardAction
        {
            public bool isFirst = false;
            public RandomChoiceActionData data;
            public CardAction action;

            public override void Begin(G g, State s, Combat c)
            {
                if (isFirst)
                {
                    data.isBlue = Random.NextDouble() >= 0.5;
                    data.isOrange = !data.isBlue;
                }
                
                if (data.isBlue)
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
                i.color = BlueColor;

                return i;
            }

            public override List<Tooltip> GetTooltips(State s)
            {
                var tooltips = action.GetTooltips(s);
                
                if (isFirst)
                    tooltips.Insert(0, new TTGlossary(glossary_item));
                
                return tooltips;
            }
        }

        internal class OrangeAction : CardAction
        {
            public RandomChoiceActionData data;
            public CardAction action;

            public override void Begin(G g, State s, Combat c)
            {
                if (data.isOrange)
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
                i.color = OrangeColor;

                return i;
            }

            public override List<Tooltip> GetTooltips(State s)
            {
                return action.GetTooltips(s);
            }
        }
    }
}