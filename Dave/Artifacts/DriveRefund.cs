using CobaltCoreModding.Definitions.ExternalItems;

namespace Dave.Artifacts;

// DriveRefund: The first time you lose overdrive each turn, draw a card.

[ArtifactMeta(pools = new[] { ArtifactPool.Common }, extraGlossary = new[] {"status.overdriveAlt"})]
public class DriveRefund : Artifact
{
    public static ExternalSprite on;
    public static ExternalSprite off;

    public bool popped;

    public override List<Tooltip> GetExtraTooltips() => new()
    {
        new TTGlossary("status.overdriveAlt")
    };

    public override void OnTurnStart(State state, Combat combat)
    {
        popped = false;
    }

    public override void OnCombatEnd(State state)
    {
        popped = false;
    }

    public override void AfterPlayerStatusAction(State state, Combat combat, Status status, AStatusMode mode, int statusAmount)
    {
        if (status != Enum.Parse<Status>("overdrive") || mode != AStatusMode.Add || statusAmount >= 0) return;

        popped = true;
        
        Pulse();
        
        combat.QueueImmediate(new ADrawCard { count = 1 });
    }

    public override Spr GetSprite()
    {
        return (Spr) (popped ? off : on).Id!;
    }
}