using System.ComponentModel.DataAnnotations;

namespace Jester.Actions;

public class PracticeAction : CardAction
{
    [Required] public bool Random;

    public override Route? BeginWithRoute(G g, State s, Combat c)
    {
        var cards = c.hand
            .Where(ca => ca.GetCurrentCost(s) > 0)
            .ToList();
        
        if (cards.Count == 0) return null;

        if (Random)
        {
            var card = ModManifest.JesterApi.GetJesterUtil().GetRandom(cards, s.rngActions);
            card.discount--;
            return null;
        }
        else
        {
            var card = cards.Last();
            card.discount--;
            return null;
        }
    }

    public override List<Tooltip> GetTooltips(State s) => new()
    {
        new TTGlossary("cardtrait.discount", 1)
    };
}