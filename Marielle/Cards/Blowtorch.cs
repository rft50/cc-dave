using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Cards;

public class Blowtorch : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("Blowtorch", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.MarielleDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Blowtorch", "name"]).Localize,
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/Blowtorch.png")).Sprite
        });
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        if (upgrade == Upgrade.A)
            return
            [
                new AStatus
                {
                    status = Status.heat,
                    statusAmount = 3,
                    targetPlayer = false
                }
            ];
        return
        [
            new AStatus
            {
                status = Status.heat,
                statusAmount = upgrade == Upgrade.B ? 4 : 3,
                targetPlayer = false
            },
            new AStatus
            {
                status = Status.heat,
                statusAmount = 1,
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