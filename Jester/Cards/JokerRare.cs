namespace Jester.Cards;

[CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class RareOffensiveJoker : AbstractJoker
{
    public RareOffensiveJoker()
    {
        Category = "offensive";
    }
}

[CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class RareDefensiveJoker : AbstractJoker
{
    public RareDefensiveJoker()
    {
        Category = "defensive";
    }
}

[CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class RareUtilityJoker : AbstractJoker
{
    public RareUtilityJoker()
    {
        Category = "utility";
    }
}