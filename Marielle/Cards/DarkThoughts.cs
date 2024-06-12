using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Cards;

public class DarkThoughts : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("DarkThoughts", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.MarielleDeck.Deck,
                rarity = Rarity.rare,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "DarkThoughts", "name"]).Localize,
            Art = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/DarkThoughts.png")).Sprite
        });
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        if (upgrade == Upgrade.A)
        {
            return
            [
                new AStatus
                {
                    status = ModEntry.Instance.Curse.Status,
                    statusAmount = 1,
                    targetPlayer = true
                },
                new ADrawCard
                {
                    count = 1
                }
            ];
        }
        return
        [
            new AStatus
            {
                status = ModEntry.Instance.Curse.Status,
                statusAmount = upgrade == Upgrade.B ? 2 : 1,
                targetPlayer = true
            }
        ];
    }

    public override CardData GetData(State state) => new()
    {
        cost = 0,
        artTint = "FFFFFF"
    };
}