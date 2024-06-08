using Dave.Actions;

namespace Dave.Cards
{
    // 2-cost, 10% boost of your choice
    // A: 1-cost
    // B: 20% boost
    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class LoadedDiceCard : Card
    {
        public static Spr red_sprite;
        public static Spr black_sprite;

        public override List<CardAction> GetActions(State s, Combat c)
        {
            var pow = upgrade == Upgrade.B ? 2 : 1;

            var list = new List<CardAction>
            {
                new BiasStatusAction { Pow = pow, disabled = flipped },
                new ADummyAction(),
                new BiasStatusAction { Pow = -pow, disabled = !flipped }
            };

            return list;
        }

        public override CardData GetData(State state) => new()
        {
            cost = upgrade == Upgrade.A ? 1 : 2,
            art = flipped ? black_sprite : red_sprite,
            floppable = true,
            exhaust = true,
            artTint = "ffffff"
        };
    }
}