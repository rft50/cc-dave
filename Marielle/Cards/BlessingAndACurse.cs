using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Cards;

public class BlessingAndACurse : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("BlessingAndACurse", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.MarielleDeck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "BlessingAndACurse", "name"]).Localize,
            Art = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/BlessingAndACurse.png")).Sprite
        });
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return
        [
            new AStatus
            {
                status = Status.serenity,
                statusAmount = upgrade switch
                {
                    Upgrade.A => 1,
                    Upgrade.B => 4,
                    _ => 2
                },
                targetPlayer = false
            },
            new AStatus
            {
                status = ModEntry.Instance.Curse.Status,
                statusAmount = upgrade == Upgrade.B ? 3 : 2,
                targetPlayer = false
            }
        ];
    }

    public override CardData GetData(State state) => new()
    {
        cost = 1,
        exhaust = true
    };
}