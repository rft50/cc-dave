using Jester.Cards;

namespace Jester.Artifacts;

[ArtifactMeta(pools = [ArtifactPool.Boss], unremovable = true)]
public class Confetti : Artifact
{
    public int Count = 0;

    public void Increment(Combat c)
    {
        Count++;
        if (Count < 3) return;
        Count = 0;
        
        var card = new Encore
        {
            exhaustOverride = false,
            singleUseOverride = true,
            temporaryOverride = true
        };
        
        c.Queue(new AAddCard
        {
            amount = 1,
            card = card,
            artifactPulse = Key(),
            destination = CardDestination.Hand
        });
    }

    public override int? GetDisplayNumber(State s) => Count;

    public override List<Tooltip>? GetExtraTooltips() =>
    [
        new TTCard
        {
            card = new Encore
            {
                exhaustOverride = false,
                singleUseOverride = true,
                temporaryOverride = true
            }
        }
    ];
}