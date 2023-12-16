namespace Jester.Cards;

// 0-Unplayable, If this is the 
[CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class CurtainCall : Card
{
    public override CardData GetData(State state) => new()
    {
        cost = 0,
        unplayable = true,
        retain = true,
        description = upgrade == Upgrade.A
            ? $"If your hand only contains Retain cards at the end of turn, Attack for <c=attack>{GetDmg(state, 1)}</c> dmg."
            : $"If this is the only card in hand at end of turn, Attack for <c=attack>{GetDmg(state, 1)}</c> dmg.",
        buoyant = upgrade == Upgrade.B
    };
}