using Dave.Cards;

namespace Dave.Artifacts;

// Ashtray: Every 5 non-Dave cards you play, add a Perfect Odds to your hand

[ArtifactMeta(pools = new[] { ArtifactPool.Common })]
public class Ashtray : Artifact
{
    public int count;

    public override List<Tooltip> GetExtraTooltips() => new()
    {
        new TTCard { card = new PerfectOddsCard() }
    };
    
    public override int? GetDisplayNumber(State s) => count;

    public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition,
        int handCount)
    {
        if (deck == (Deck)ModManifest.dave_deck.Id) return;

        count++;

        if (count < 5) return;

        count -= 5;
        
        combat.QueueImmediate(new AAddCard
        {
            card = new PerfectOddsCard(),
            destination = CardDestination.Hand,
            amount = 1,
            artifactPulse = Key()
        });
    }
}