namespace Jester.Cards;

[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class Joker0Offensive : AbstractJoker
{
    public Joker0Offensive()
    {
        Energy = 0;
        Category = "offensive";
    }
}

[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class Joker1Offensive : AbstractJoker
{
    public Joker1Offensive()
    {
        Energy = 1;
        Category = "offensive";
    }
}

[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class Joker2Offensive : AbstractJoker
{
    public Joker2Offensive()
    {
        Energy = 2;
        Category = "offensive";
    }
}

[CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class Joker3Offensive : AbstractJoker
{
    public Joker3Offensive()
    {
        Energy = 3;
        Category = "offensive";
    }
}

[CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class Joker4Offensive : AbstractJoker
{
    public Joker4Offensive()
    {
        Energy = 4;
        Category = "offensive";
    }
}