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
            new RandomChoiceSetupAction { data = data }
        };
            
        if (red != null)
            actions.AddRange(red.Select(t => ModManifest.KokoroApi.ConditionalActions.Make(
                new RedBlackCondition { isRed = true, data = data }, t
            )));
            
        if (black != null)
            actions.AddRange(black.Select(t => ModManifest.KokoroApi.ConditionalActions.Make(
                new RedBlackCondition { isRed = false, data = data }, t
            )));

        return actions;
    }

    public class RandomChoiceSetupAction : CardAction
    {
        public RandomChoiceActionData data;

        public override void Begin(G g, State s, Combat c)
        {
            var isRoll = false;
                
            var redOdds = 5f;
            redOdds += s.ship.Get((Status)(ModManifest.red_bias?.Id ?? throw new Exception()));
            redOdds -= s.ship.Get((Status)(ModManifest.black_bias?.Id ?? throw new Exception()));
            redOdds /= 10;

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
                data.isRed = s.rngActions.Next() < redOdds;
                data.isBlack = !data.isRed;
                isRoll = true;
            }

            data.filled = true;
                
            ModManifest.EventHub.SignalEvent("Dave.RedBlackRoll", Tuple.Create(s, c, data.isRed, data.isBlack, isRoll));
                
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
    public bool isRed;
    public bool isBlack;
    public bool filled = false;
}

public class RedBlackCondition : IKokoroApi.IConditionalActionApi.IBoolExpression
{
    public RandomChoiceActionData data;
    public bool isRed;
    
    public void Render(G g, ref Vec position, bool isDisabled, bool dontRender)
    {
        if (!dontRender)
        {
            Draw.Sprite(
                (Spr) (isRed ? ModManifest.red_sprite!.Id! : ModManifest.black_sprite!.Id!),
                position.x,
                position.y,
                color: isDisabled ? Colors.disabledIconTint : Colors.white
            );
        }

        position.x += 8;
    }

    public string GetTooltipDescription(State state, Combat? combat)
    {
        return isRed ? "on Red" : "on Black";
    }

    public bool GetValue(State state, Combat combat)
    {
        if (data.filled) return isRed ? data.isRed : data.isBlack;

        var redStatus = state.ship.Get((Status)ModManifest.red_rigging!.Id!);
        var blackStatus = state.ship.Get((Status)ModManifest.black_rigging!.Id!);
        
        return isRed ?
            redStatus > 0 || blackStatus == 0 :
            blackStatus > 0 || redStatus == 0;
    }

    public bool ShouldRenderQuestionMark(State state, Combat? combat) => false;
}