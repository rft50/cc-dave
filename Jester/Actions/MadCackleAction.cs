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
            typeof(CommonOffensiveJoker), typeof(CommonOffensiveJoker), typeof(CommonOffensiveJoker), typeof(UncommonOffensiveJoker), typeof(UncommonOffensiveJoker), typeof(RareOffensiveJoker),
            typeof(CommonDefensiveJoker), typeof(CommonDefensiveJoker), typeof(CommonDefensiveJoker), typeof(UncommonDefensiveJoker), typeof(UncommonDefensiveJoker), typeof(RareDefensiveJoker),
            typeof(CommonUtilityJoker), typeof(CommonUtilityJoker), typeof(CommonUtilityJoker), typeof(UncommonUtilityJoker), typeof(UncommonUtilityJoker), typeof(RareUtilityJoker)
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