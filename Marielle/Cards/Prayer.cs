using System;
using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Cards;

public class Prayer : Card, IRegisterable
{
    public bool Discounted = false;
    
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("Prayer", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.MarielleDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Prayer", "name"]).Localize,
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/Prayer.png")).Sprite
        });
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return
        [
            new AStatus
            {
                status = Status.evade,
                statusAmount = upgrade == Upgrade.A ? 2 : 1,
                targetPlayer = true
            },
            new AStatus
            {
                status = Status.serenity,
                statusAmount = upgrade == Upgrade.B ? 2 : 1,
                targetPlayer = true
            }
        ];
    }

    public override CardData GetData(State state) => new()
    {
        cost = Discounted ? 1 : 2,
        artTint = "FFFFFF"
    };
}