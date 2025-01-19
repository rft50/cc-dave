namespace Jester.Artifacts;

[ArtifactMeta(pools = [ArtifactPool.Common])]
public class JingleBells : Artifact
{
    public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition,
        int handCount)
    {
        var count = CardPlayTracker.GetCardPlaysThisTurn(state, combat).Count(c => c.uuid == card.uuid);

        if (count == 1)
        {
            combat.Queue(new AEnergy
            {
                changeAmount = 1,
                artifactPulse = Key()
            });
        }
    }
}