using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Cards;

public class Malediction : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("Malediction", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.MarielleDeck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Malediction", "name"]).Localize,
            Art = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/Malediction.png")).Sprite
        });
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        var curse = ModEntry.Instance.Curse.Status;
        if (upgrade == Upgrade.A)
        {
            return
            [
                new AStatus
                {
                    status = curse,
                    statusAmount = 1,
                    targetPlayer = true
                },
                new AVariableHint
                {
                    status = curse
                },
                new AAttack
                {
                    damage = GetDamage(s),
                    xHint = 1
                }
            ];
        }
        return
        [
            new AVariableHint
            {
                status = curse
            },
            new AAttack
            {
                damage = GetDamage(s),
                xHint = 1
            }
        ];
    }

    public override CardData GetData(State state) => new()
    {
        cost = upgrade == Upgrade.B ? 1 : 2,
        description = upgrade == Upgrade.B ? string.Format(ModEntry.Instance.AnyLocalizations
            .Bind(["card", "Malediction", "description", "b"]).Localize("en") ?? "look out for Soggoru's replacement for this {0}", GetDamage(state)) : null
    };

    private int GetDamage(State s)
    {
        var dmg = s.ship.Get(ModEntry.Instance.Curse.Status);
        switch (upgrade)
        {
            case Upgrade.A:
                dmg += 1 + s.ship.Get(Status.boost);
                break;
            case Upgrade.B:
                dmg -= s.ship.Get(Status.heat);
                break;
        }
        return GetDmg(s, dmg);
    }
}