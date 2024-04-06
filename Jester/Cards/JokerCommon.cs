namespace Jester.Cards;

[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class CommonOffensiveJoker : AbstractJoker
{
    public CommonOffensiveJoker()
    {
        Category = "offensive";
    }
}

[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class CommonDefensiveJoker : AbstractJoker
{
    public CommonDefensiveJoker()
    {
        Category = "defensive";
    }
}

[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class CommonUtilityJoker : AbstractJoker
{
    public CommonUtilityJoker()
    {
        Category = "utility";
    }
}