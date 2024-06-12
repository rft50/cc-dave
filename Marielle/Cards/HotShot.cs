using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Cards;

public class HotShot : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("HotShot", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.MarielleDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "HotShot", "name"]).Localize,
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/HotShot.png")).Sprite
        });
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        if (upgrade == Upgrade.B)
        {
            return
            [
                new AAttack
                {
                    damage = GetDmg(s, 1)
                },
                new AStatus
                {
                    status = Status.heat,
                    statusAmount = 3,
                    targetPlayer = false
                }
            ];
        }
        return
        [
            new AAttack
            {
                damage = GetDmg(s, upgrade == Upgrade.A ? 2 : 1),
                status = Status.heat,
                statusAmount = 2
            }
        ];
    }

    public override CardData GetData(State state) => new()
    {
        cost = 1,
        artTint = "FFFFFF"
    };
}