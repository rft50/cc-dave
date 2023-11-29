using Dave.Cards;

namespace Dave.Artifacts;

// Rigged Dice: Every other turn, add a Perfect Odds to your hand

[ArtifactMeta(pools = new[] { ArtifactPool.Boss }, unremovable = true)]
public class RiggedDice : Artifact
{
    public int count;
    
    public override int? GetDisplayNumber(State s) => count;
    
    public override List<Tooltip> GetExtraTooltips() => new()
    {
        new TTCard { card = new PerfectOddsCard() }
    };
    
    public override void OnTurnStart(State state, Combat combat)
    {
        count++;
        if (count < 2)
            return;
        count -= 2;
        
        combat.QueueImmediate(new AAddCard
        {
            card = new PerfectOddsCard(),
            destination = CardDestination.Hand,
            amount = 1,
            artifactPulse = Key()
        });
    }
}