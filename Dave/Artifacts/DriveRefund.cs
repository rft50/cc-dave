namespace Dave.Artifacts;

// DriveRefund: The first time you lose overdrive each turn, draw a card.

[ArtifactMeta(pools = new[] { ArtifactPool.Common }, extraGlossary = new[] {"status.overdriveAlt"})]
public class DriveRefund : Artifact
{
    internal static Spr On;
    internal static Spr Off;

    private bool _popped;

    public override List<Tooltip> GetExtraTooltips() => new()
    {
        new TTGlossary("status.overdriveAlt")
    };

    public override void OnTurnStart(State state, Combat combat)
    {
        _popped = false;
    }

    public override void OnCombatEnd(State state)
    {
        _popped = false;
    }

    public override void AfterPlayerStatusAction(State state, Combat combat, Status status, AStatusMode mode, int statusAmount)
    {
        if (status != Enum.Parse<Status>("overdrive") || mode != AStatusMode.Add || statusAmount >= 0) return;

        _popped = true;
        
        Pulse();
        
        combat.QueueImmediate(new ADrawCard { count = 1 });
    }

    public override Spr GetSprite()
    {
        return _popped ? Off : On;
    }
}