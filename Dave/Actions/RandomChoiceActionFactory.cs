using Dave.Artifacts;
using Dave.External;

namespace Dave.Actions;

public class RandomChoiceActionFactory
{
    private static readonly Random Random = new();
    public static string glossary_item = "";
        
    // 0th element is a setup action that should be the first action
    // after that it's all the reds, then all the blacks
    public static List<CardAction> BuildActions(List<CardAction>? red, List<CardAction>? black = null)
    {
        var data = new RandomChoiceActionData();

        var actions = new List<CardAction>
        {
            new RandomChoiceSetupAction { Data = data }
        };
            
        if (red != null)
            actions.AddRange(red.Select(t => ModEntry.Instance.KokoroApi.ConditionalActions.Make(
                new RedBlackCondition { IsRed = true, Data = data }, t
            )));
            
        if (black != null)
            actions.AddRange(black.Select(t => ModEntry.Instance.KokoroApi.ConditionalActions.Make(
                new RedBlackCondition { IsRed = false, Data = data }, t
            )));

        return actions;
    }

    public class RandomChoiceSetupAction : CardAction
    {
        public RandomChoiceActionData Data = null!;

        public override void Begin(G g, State s, Combat c)
        {
            var isRoll = false;
                
            var redOdds = 5f;
            redOdds += s.ship.Get(ModEntry.Instance.RedBias.Status);
            redOdds -= s.ship.Get(ModEntry.Instance.BlackBias.Status);
            redOdds /= 10;

            if (s.ship.Get(ModEntry.Instance.RedRigging.Status) > 0)
            {
                Data.IsRed = true;
                c.QueueImmediate(new AStatus { status = ModEntry.Instance.RedRigging.Status, targetPlayer = true, statusAmount = -1, mode = AStatusMode.Add });
            }

            if (s.ship.Get(ModEntry.Instance.BlackRigging.Status) > 0)
            {
                Data.IsBlack = true;
                c.QueueImmediate(new AStatus { status = ModEntry.Instance.BlackRigging.Status, targetPlayer = true, statusAmount = -1, mode = AStatusMode.Add });
            }

            if (Data is { IsRed: false, IsBlack: false })
            {
                Data.IsRed = s.rngActions.Next() < redOdds;
                Data.IsBlack = !Data.IsRed;
                isRoll = true;
            }

            Data.Filled = true;
            
            foreach (var rollHook in ModEntry.Instance.RollManager.ToList())
            {
                rollHook.OnRoll(s, c, Data.IsRed, Data.IsBlack, isRoll);
            }
                
            if (!ArtifactUtil.PlayerHasArtifactOfType(s, typeof(Chip)))
                c.QueueImmediate(new AAddArtifact { artifact = new Chip() });

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
}

public class RandomChoiceActionData
{
    public bool IsRed;
    public bool IsBlack;
    public bool Filled;
}

public class RedBlackCondition : IKokoroApi.IConditionalActionApi.IBoolExpression
{
    internal static Spr Red;
    internal static Spr Black;
    public RandomChoiceActionData Data = null!;
    public bool IsRed;
    
    public void Render(G g, ref Vec position, bool isDisabled, bool dontRender)
    {
        if (!dontRender)
        {
            Draw.Sprite(
                IsRed ? Red : Black,
                position.x,
                position.y,
                color: isDisabled ? Colors.disabledIconTint : Colors.white
            );
        }

        position.x += 8;
    }

    public string GetTooltipDescription(State state, Combat? combat)
    {
        return IsRed ? "on Red" : "on Black";
    }

    public bool GetValue(State state, Combat combat)
    {
        if (Data.Filled) return IsRed ? Data.IsRed : Data.IsBlack;

        var redStatus = state.ship.Get(ModEntry.Instance.RedRigging.Status);
        var blackStatus = state.ship.Get(ModEntry.Instance.BlackRigging.Status);
        
        return IsRed ?
            redStatus > 0 || blackStatus == 0 :
            blackStatus > 0 || redStatus == 0;
    }

    public bool ShouldRenderQuestionMark(State state, Combat? combat) => false;
}