using Jester.Cards;
using Jester.Patches;

namespace Jester.Actions;

public class EncoreAction : CardAction
{
    public bool ReturnCard;
    
    public override Route? BeginWithRoute(G g, State s, Combat c)
    {
        var cards = CardPlayTracker.GetCardPlaysThisTurn(s, c)
            .Where(ca => ca.GetType() != typeof(Encore))
            .DistinctBy(ca => ca.uuid)
            .ToList();

        if (cards.Count == 0) return null;
        
        CardBrowsePatch.CardsToInject = cards;
        
        var cardBrowse = new CardBrowse
        {
            mode = CardBrowse.Mode.Browse,
            browseAction = new EncoreSubAction
            {
                ReturnCard = ReturnCard
            }
        };

        timer = 0;

        return cardBrowse.GetCardList(g).Count != 0 ? cardBrowse : null;

    }
}

public class EncoreSubAction : CardAction
{
    public bool ReturnCard;
    
    public override void Begin(G g, State s, Combat c)
    {
        if (selectedCard == null) return;

        if (ReturnCard)
        {
            s.RemoveCardFromWhereverItIs(selectedCard.uuid);
            c.SendCardToHand(s, selectedCard);
        }

        timer = 0;
        
        c.QueueImmediate(selectedCard.GetActions(s, c));
    }
}