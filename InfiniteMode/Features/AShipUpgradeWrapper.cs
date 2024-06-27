using System.Collections.Generic;

namespace InfiniteMode.Features;

public class AShipUpgradeWrapper : CardAction
{
    public CardAction Action = null!;
    public string Description = "";

    public override List<Tooltip> GetTooltips(State s) => Action.GetTooltips(s);
    public override bool CanSkipTimerIfLastEvent() => Action.CanSkipTimerIfLastEvent();

    public override void Begin(G g, State s, Combat c)
    {
        Action.Begin(g, s, c);
        timer = Action.timer;
    }
    public override Route? BeginWithRoute(G g, State s, Combat c) => Action.BeginWithRoute(g, s, c);
    public override void Update(G g, State s, Combat c)
    {
        Action.Update(g, s, c);
        timer = Action.timer;
    }
    public override Icon? GetIcon(State s) => Action.GetIcon(s);
    public override string GetUpgradeText(State s) => Description;
    public override string? GetCardSelectText(State s) => Action.GetCardSelectText(s);
}

public static class AShipUpgradeWrapperExtensions
{
    public static CardAction WithDescription(this CardAction action, string description)
    {
        return new AShipUpgradeWrapper
        {
            Action = action,
            Description = description
        };
    }
}