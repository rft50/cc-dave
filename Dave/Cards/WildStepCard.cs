using Dave.Actions;

namespace Dave.Cards
{
    // 1-cost, move enemy 1, move self 1 (both random)
    // A: 0-cost
    // B: 2 distance on enemy, you gain Evade
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class WildStepCard : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            var list = new List<CardAction>();

            if (upgrade == Upgrade.B)
            {
                list.Add(new RandomMoveFoeAction { Dist = 2 });
                list.Add(new AStatus { status = Status.evade, statusAmount = 1, targetPlayer = true });
            }
            else
            {
                list.Add(new RandomMoveFoeAction { Dist = 1 });
                list.Add(new AMove { dir = 1, targetPlayer = true, isRandom = true });
            }
            
            return list;
        }
        
        public override CardData GetData(State state) => new()
        {
            cost = upgrade == Upgrade.A ? 0 : 1
        };
    }
}