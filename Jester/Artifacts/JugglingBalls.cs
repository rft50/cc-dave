namespace Jester.Artifacts;

[ArtifactMeta(pools = [ArtifactPool.Boss], unremovable = true)]
public class JugglingBalls : Artifact
{
    internal static Spr ReadySpr;
    internal static Spr NotReadySpr;
    
    public bool Ready = true;

    public override void OnTurnStart(State state, Combat combat)
    {
        Ready = true;
    }

    public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition,
        int handCount)
    {
        if (!Ready || deck != (Deck)ModManifest.JesterDeck!.Id!) return;
        Ready = false;
        combat.Queue(new AEnergy
        {
            changeAmount = 1,
            artifactPulse = Key()
        });
    }

    public override Spr GetSprite()
    {
        return Ready ? ReadySpr : NotReadySpr;
    }
}