using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Cards;

public class FireField : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("FireField", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.MarielleDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "FireField", "name"]).Localize,
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/FireField.png")).Sprite
        }); 
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        if (upgrade == Upgrade.B)
        {
            return
            [
                new AStatus
                {
                    status = Status.shield,
                    statusAmount = 2,
                    targetPlayer = true
                },

                new AStatus
                {
                    status = Status.heat,
                    statusAmount = 3,
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
        else
        {
            return
            [
                new AStatus
                {
                    status = Status.shield,
                    statusAmount = upgrade == Upgrade.A ? 2 : 1,
                    targetPlayer = true
                },

                new AStatus
                {
                    status = Status.heat,
                    statusAmount = 2,
                    targetPlayer = false
                }
            ];
        }
    }

    public override CardData GetData(State state) => new()
    {
        cost = 1
    };
}