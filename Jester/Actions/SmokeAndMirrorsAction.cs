namespace Jester.Actions;

public class SmokeAndMirrorsAction : CardAction
{
    public override void Begin(G g, State s, Combat c)
    {
        foreach (var card in c.hand)
        {
            var origCost = card.GetData(s).cost;
            var destCost = s.rngActions.NextInt() % 4;

            card.discount = destCost - origCost;
        }
    }
}