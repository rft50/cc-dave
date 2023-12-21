using Jester.Patches;

namespace Jester.Actions;

public class ArbitraryCardPlayAction : CardAction
{
    public Card Card = null!;
    public bool DoCardCheck = true;
    public bool DoCardDisposal = true;
    
    public override void Begin(G g, State s, Combat c)
    {
        CombatPatch.DoCardCheck = DoCardCheck;
        CombatPatch.DoCardDisposal = DoCardDisposal;

        c.TryPlayCard(s, Card, true);
    }
}