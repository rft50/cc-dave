using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Cards;

public class Voodoo : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("Voodoo", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.MarielleDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Voodoo", "name"]).Localize,
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/Voodoo.png")).Sprite
        });
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return
        [
            new AAttack
            {
                damage = GetDmg(s, upgrade == Upgrade.None ? 3 : 6),
                status = ModEntry.Instance.Curse.Status,
                statusAmount = upgrade == Upgrade.B ? 2 : 1
            },
        ];
    }

    public override CardData GetData(State state) => new()
    {
        cost = upgrade == Upgrade.B ? 2 : 1,
        artTint = "FFFFFF",
        exhaust = true
    };
}