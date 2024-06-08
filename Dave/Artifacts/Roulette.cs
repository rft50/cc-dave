using Dave.Api;
using Dave.Cards;

namespace Dave.Artifacts;

// Roulette: Every 3 red/black cards you play with no Rigging, add a Perfect Odds to your hand

[ArtifactMeta(pools = new[] { ArtifactPool.Common })]
public class Roulette : Artifact, IRollHook
{
    public int Count;

    public Roulette()
    {
        ModEntry.Instance.RollManager.Register(this, 0);
    }

    public override void OnRemoveArtifact(State state)
    {
        ModEntry.Instance.RollManager.Unregister(this);
    }

    public override List<Tooltip>? GetExtraTooltips() => new()
    {
        new TTCard { card = new PerfectOddsCard() }
    };
    
    public override int? GetDisplayNumber(State s) => Count;

    public void OnRoll(State state, Combat combat, bool isRed, bool isBlack, bool isRoll)
    {
        if (!ArtifactUtil.PlayerHasArtifact(state, this))
        {
            ModEntry.Instance.RollManager.Unregister(this);
            return;
        }

        if (!isRoll) return;

        Count++;

        if (Count < 3) return;

        Count -= 3;
        
        combat.QueueImmediate(new AAddCard
        {
            card = new PerfectOddsCard(),
            destination = CardDestination.Hand,
            amount = 1,
            artifactPulse = Key()
        });
    }
}