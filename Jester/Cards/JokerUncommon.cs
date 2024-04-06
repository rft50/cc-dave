namespace Jester.Cards;

[CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class UncommonOffensiveJoker : AbstractJoker
{
    public UncommonOffensiveJoker()
    {
        Category = "offensive";
    }
}

[CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class UncommonDefensiveJoker : AbstractJoker
{
    public UncommonDefensiveJoker()
    {
        Category = "defensive";
    }
}

[CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class UncommonUtilityJoker : AbstractJoker
{
    public UncommonUtilityJoker()
    {
        Category = "utility";
    }
}