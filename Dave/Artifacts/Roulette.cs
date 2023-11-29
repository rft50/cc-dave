using Dave.Cards;

namespace Dave.Artifacts;

// Roulette: Every 3 red/black cards you play with no Rigging, add a Perfect Odds to your hand

[ArtifactMeta(pools = new[] { ArtifactPool.Common })]
public class Roulette : Artifact
{
    public int count;

    public Roulette()
    {
        if (ModManifest.EventHub != null)
            ModManifest.EventHub.ConnectToEvent<Tuple<State, Combat, bool, bool, bool>>("Dave.RedBlackRoll", OnRoll);
    }

    public override void OnRemoveArtifact(State state)
    {
        ModManifest.EventHub.DisconnectFromEvent<Tuple<State, Combat, bool, bool, bool>>("Dave.RedBlackRoll", OnRoll);
    }

    public override List<Tooltip>? GetExtraTooltips() => new()
    {
        new TTCard { card = new PerfectOddsCard() }
    };
    
    public override int? GetDisplayNumber(State s) => count;

    private void OnRoll(Tuple<State, Combat, bool, bool, bool> data)
    {
        var (state, combat, _, _, isRoll) = data;

        if (!ArtifactUtil.PlayerHasArtifact(state, this))
        {
            ModManifest.EventHub.DisconnectFromEvent<Tuple<State, Combat, bool, bool, bool>>("Dave.RedBlackRoll", OnRoll);
            return;
        }

        if (!isRoll) return;

        count++;

        if (count < 3) return;

        count -= 3;
        
        combat.QueueImmediate(new AAddCard
        {
            card = new PerfectOddsCard(),
            destination = CardDestination.Hand,
            amount = 1,
            artifactPulse = Key()
        });
    }
}