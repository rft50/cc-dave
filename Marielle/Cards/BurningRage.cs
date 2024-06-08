using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Cards;

public class BurningRage : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("BurningRage", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.MarielleDeck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "BurningRage", "name"]).Localize,
            Art = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/BurningRage.png")).Sprite
        });
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return
        [
            new AAttack
            {
                damage = GetDmg(s, 4),
            },
            new AStatus
            {
                status = Status.heat,
                statusAmount = upgrade == Upgrade.B ? 1 : 2,
                targetPlayer = true
            }
        ];
    }

    public override CardData GetData(State state) => new()
    {
        cost = upgrade == Upgrade.A ? 0 : 1
    };
}