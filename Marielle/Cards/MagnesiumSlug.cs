using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Cards;

public class MagnesiumSlug : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("MagnesiumSlug", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.MarielleDeck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "MagnesiumSlug", "name"]).Localize,
            Art = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/MagnesiumSlug.png")).Sprite
        });
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return
        [
            new AAttack
            {
                damage = GetDmg(s, upgrade == Upgrade.A ? 3 : 2),
                status = Status.heat,
                statusAmount = upgrade == Upgrade.A ? 7 : 6
            },
            new AStatus
            {
                status = Status.heat,
                statusAmount = upgrade == Upgrade.B ? 2 : 1,
                targetPlayer = true
            }
        ];
    }

    public override CardData GetData(State state) => new()
    {
        cost = upgrade == Upgrade.B ? 1 : 2
    };
}