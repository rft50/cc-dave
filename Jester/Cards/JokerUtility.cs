namespace Jester.Cards;

[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class Joker0Utility : AbstractJoker
{
    public Joker0Utility()
    {
        Energy = 0;
        Category = "utility";
    }
}

[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class Joker1Utility : AbstractJoker
{
    public Joker1Utility()
    {
        Energy = 1;
        Category = "utility";
    }
}

[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class Joker2Utility : AbstractJoker
{
    public Joker2Utility()
    {
        Energy = 2;
        Category = "utility";
    }
}

[CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class Joker3Utility : AbstractJoker
{
    public Joker3Utility()
    {
        Energy = 3;
        Category = "utility";
    }
}

[CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class Joker4Utility : AbstractJoker
{
    public Joker4Utility()
    {
        Energy = 4;
        Category = "utility";
    }
}