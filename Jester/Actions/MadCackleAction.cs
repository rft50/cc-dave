using FMOD.Studio;
using Jester.Cards;
using Util = Jester.Generator.Util;

namespace Jester.Actions;

public class MadCackleAction : CardAction
{
    public int OfferCount;
    
    public override Route BeginWithRoute(G g, State s, Combat c)
    {
        var cards = new List<Type>
        {
            typeof(Joker0Offensive), typeof(Joker1Offensive), typeof(Joker2Offensive), typeof(Joker3Offensive), typeof(Joker4Offensive),
            typeof(Joker0Defensive), typeof(Joker1Defensive), typeof(Joker2Defensive), typeof(Joker3Defensive), typeof(Joker4Defensive),
            typeof(Joker0Utility), typeof(Joker1Utility), typeof(Joker2Utility), typeof(Joker3Utility), typeof(Joker4Utility)
        };
        if (OfferCount > cards.Count)
            OfferCount = cards.Count;

        ModManifest.JesterApi.GetJesterUtil().Shuffle(cards, s.rngActions.Offshoot());

        var offers = cards.Take(OfferCount)
            .Select(Activator.CreateInstance)
            .OfType<AbstractJoker>()
            .ToList();

        foreach (var card in offers)
        {
            card.temporaryOverride = true;
            card.SingleUse = true;
        }
        
        timer = 0;
        
        return new CardReward
        {
            canSkip = false,
            cards = offers.OfType<Card>().ToList()
        };
    }
}