using Jester.Actions;
using Jester.Cards;

namespace Jester.Artifacts;

[ArtifactMeta(pools = [ArtifactPool.Common])]
public class CherishedPhoto : Artifact
{
    public override void OnCombatEnd(State state)
    {
        state.rewardsQueue.Add(new CherishedPhotoAction());
    }
}

internal class CherishedPhotoAction : CardAction
{
    
    public override Route? BeginWithRoute(G g, State s, Combat c)
    {
        var cards = s.deck
            .Where(ca => ca is AbstractJoker)
            .ToList();

        if (cards.Count == 0) return null;
        
        var cardBrowse = new ArbitraryCardBrowse
        {
            mode = CardBrowse.Mode.Browse,
            browseAction = new CherishedPhotoSubAction(),
            Cards = cards
        };

        timer = 0;

        return cardBrowse.GetCardList(g).Count != 0 ? cardBrowse : null;
    }
}

internal class CherishedPhotoSubAction : CardAction
{
    public override Route? BeginWithRoute(G g, State s, Combat c)
    {
        if (selectedCard is not AbstractJoker card) return null;

        s.deck.Remove(card);

        List<Card> cards =
        [
            card,
            card.CopyWithNewId(),
            card.CopyWithNewId(),
            card.CopyWithNewId(),
            card.CopyWithNewId()
        ];

        for (var i = 1; i < 5; i++)
        {
            var crd = cards[i] as AbstractJoker;
            crd!.Seed = s.rngCardOfferings.NextInt();
        }
        
        return new CardReward
        {
            canSkip = false,
            cards = cards
        };
    }
}