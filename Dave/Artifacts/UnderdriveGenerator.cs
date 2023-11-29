namespace Dave.Artifacts;

// Underdrive Generator: +1 Energy, every other turn, gain -1 overdrive

[ArtifactMeta(pools = new[] { ArtifactPool.Boss }, unremovable = true)]
public class UnderdriveGenerator : Artifact
{
    public int count;
    
    public override void OnReceiveArtifact(State state) => ++state.ship.baseEnergy;

    public override void OnRemoveArtifact(State state) => --state.ship.baseEnergy;

    public override int? GetDisplayNumber(State s) => count;

    public override List<Tooltip> GetExtraTooltips() => new()
    {
        new TTGlossary("status.overdrive", -1)
    };

    public override void OnTurnStart(State state, Combat combat)
    {
        count++;
        if (count < 2)
            return;
        count -= 2;
        
        combat.QueueImmediate(new AStatus
        {
            status = Enum.Parse<Status>("overdrive"),
            statusAmount = -1,
            targetPlayer = true
        });
    }
}