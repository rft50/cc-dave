using Dave.Api;
using Dave.Artifacts;
using Dave.External;
using Nickel;

namespace Dave.Actions;

public static class RandomChoiceActionFactory
{
    // 0th element is a setup action that should be the first action
    // after that it's all the reds, then all the blacks
    public static List<CardAction> BuildActions(List<CardAction>? red, List<CardAction>? black = null)
    {
        var guid = Guid.NewGuid();

        var actions = new List<CardAction>
        {
            ModEntry.Instance.KokoroApi.Actions.MakeHidden(new RandomChoiceSetupAction { Guid = guid })
        };
            
        if (red != null)
            actions.AddRange(red.Select(t => ModEntry.Instance.KokoroApi.ConditionalActions.Make(
                new RedBlackCondition { IsRed = true, Guid = guid }, t
            )));
            
        if (black != null)
            actions.AddRange(black.Select(t => ModEntry.Instance.KokoroApi.ConditionalActions.Make(
                new RedBlackCondition { IsRed = false, Guid = guid }, t
            )));

        return actions;
    }

    public static CardAction MakeSetupAction(Guid guid)
    {
        return ModEntry.Instance.KokoroApi.Actions.MakeHidden(new RandomChoiceSetupAction { Guid = guid });
    }
    
    public static CardAction MakeRedBlackAction(Guid guid, bool isRed, CardAction action)
    {
        return ModEntry.Instance.KokoroApi.ConditionalActions.Make(
            new RedBlackCondition { IsRed = isRed, Guid = guid }, action
        );
    }

    public static RandomChoiceActionData RandomRoll(State s, Combat c, int bias = 0)
    {
        var data = new RandomChoiceActionData();

        var info = ModEntry.Instance.RollModifierManager
            .Select(d => d.ModifyRoll(s, c))
            .FirstOrDefault(d => d != null);
        
        data.IsRed = info?.Item1 ?? false;
        data.IsBlack = info?.Item2 ?? false;

        if (data is { IsRed: false, IsBlack: false })
        {
            PureRandomRoll(s, data, bias);
        }
            
        foreach (var rollHook in ModEntry.Instance.RollHookManager.ToList())
        {
            rollHook.OnRoll(s, c, data.IsRed, data.IsBlack, data.IsRoll);
        }

        return data;
    }

    public static void PureRandomRoll(State s, RandomChoiceActionData data, int bias = 0)
    {
        var redOdds = 5f + bias;
        redOdds += s.ship.Get(ModEntry.Instance.RedBias.Status);
        redOdds -= s.ship.Get(ModEntry.Instance.BlackBias.Status);
        redOdds /= 10;
        
        data.IsRed = s.rngActions.Next() < redOdds;
        data.IsBlack = !data.IsRed;
        data.IsRoll = true;
    }

    private class RandomChoiceSetupAction : CardAction
    {
        internal Guid Guid;

        public override void Begin(G g, State s, Combat c)
        {
            var data = RandomRoll(s, c);
            var allData =
                ModEntry.Instance.Helper.ModData.ObtainModData<Dictionary<Guid, RandomChoiceActionData>>(s, "DaveRandomFlags",
                    () => []);
            allData[Guid] = data;
            ModEntry.Instance.Helper.ModData.SetModData(s, "DaveRandomFlags", allData);
                
            if (!ArtifactUtil.PlayerHasArtifactOfType(s, typeof(Chip)))
                c.QueueImmediate(new AAddArtifact { artifact = new Chip() });

            c.Queue(new RandomChoiceTeardownAction { Guid = Guid });
            
            timer = 0;
        }

        public override Icon? GetIcon(State s)
        {
            return null;
        }

        public override List<Tooltip> GetTooltips(State s)
        {
            return
            [
                new GlossaryTooltip("Dave::action::RedBlack")
                {
                    Title = ModEntry.Instance.Localizations.Localize(["action", "RedBlack", "name"]),
                    Description = ModEntry.Instance.Localizations.Localize(["action", "RedBlack", "description"]),
                    TitleColor = Colors.action,
                    Icon = RedBlackCondition.Red
                }
            ];
        }
    }

    private class RandomChoiceTeardownAction : CardAction
    {
        internal Guid Guid;
        public override void Begin(G g, State s, Combat c)
        {
            var allData =
                ModEntry.Instance.Helper.ModData.ObtainModData<Dictionary<Guid, RandomChoiceActionData>>(s, "DaveRandomFlags",
                    () => []);
            allData.Remove(Guid);
            ModEntry.Instance.Helper.ModData.SetModData(s, "DaveRandomFlags", allData);
        }
    }
}

public class RandomChoiceActionData
{
    public bool IsRed;
    public bool IsBlack;
    public bool IsRoll;
    public Guid Guid;
}

public class RedBlackCondition : IKokoroApi.IConditionalActionApi.IBoolExpression
{
    internal static Spr Red;
    internal static Spr Black;
    internal Guid Guid;
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
        // return false;
        var allData =
            ModEntry.Instance.Helper.ModData.ObtainModData<Dictionary<Guid, RandomChoiceActionData>>(state, "DaveRandomFlags",
                () => []);
        allData.TryGetValue(Guid, out var data);
        if (data != null) return IsRed ? data.IsRed : data.IsBlack;

        var redStatus = state.ship.Get(ModEntry.Instance.RedRigging.Status);
        var blackStatus = state.ship.Get(ModEntry.Instance.BlackRigging.Status);
        
        return IsRed ?
            redStatus > 0 || blackStatus == 0 :
            blackStatus > 0 || redStatus == 0;
    }

    public bool ShouldRenderQuestionMark(State state, Combat? combat) => false;
}

public class RiggingRollModifier : IDaveApi.IRollModifier
{
    public (bool, bool)? ModifyRoll(State state, Combat combat)
    {
        var isRed = false;
        var isBlack = false;

        if (state.ship.Get(ModEntry.Instance.RedRigging.Status) > 0)
        {
            isRed = true;
            combat.QueueImmediate(new AStatus { status = ModEntry.Instance.RedRigging.Status, targetPlayer = true, statusAmount = -1, mode = AStatusMode.Add, timer = 0 });
        }

        if (state.ship.Get(ModEntry.Instance.BlackRigging.Status) > 0)
        {
            isBlack = true;
            combat.QueueImmediate(new AStatus { status = ModEntry.Instance.BlackRigging.Status, targetPlayer = true, statusAmount = -1, mode = AStatusMode.Add, timer = 0 });
        }
        
        if (isRed || isBlack) return (isRed, isBlack);
        return null;
    }
}