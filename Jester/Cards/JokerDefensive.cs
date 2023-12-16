namespace Jester.Cards;

[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class Joker0Defensive : AbstractJoker
{
    public Joker0Defensive()
    {
        Energy = 0;
        Category = "defensive";
    }
}

[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class Joker1Defensive : AbstractJoker
{
    public Joker1Defensive()
    {
        Energy = 1;
        Category = "defensive";
    }
}

[CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class Joker2Defensive : AbstractJoker
{
    public Joker2Defensive()
    {
        Energy = 2;
        Category = "defensive";
    }
}

[CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class Joker3Defensive : AbstractJoker
{
    public Joker3Defensive()
    {
        Energy = 3;
        Category = "defensive";
    }
}

[CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class Joker4Defensive : AbstractJoker
{
    public Joker4Defensive()
    {
        Energy = 4;
        Category = "defensive";
    }
}