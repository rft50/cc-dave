using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Cards;

public class Hex : Card, IRegisterable, IHasCustomCardTraits
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("Hex", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.MarielleDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Hex", "name"]).Localize,
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/Hex.png")).Sprite
        });
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return
        [
            new AStatus
            {
                status = ModEntry.Instance.Curse.Status,
                statusAmount = upgrade == Upgrade.B ? 2 : 1,
                targetPlayer = false
            },
            new AStatus
            {
                status = Status.tempShield,
                statusAmount = 2,
                targetPlayer = true
            }
        ];
    }

    public override CardData GetData(State state) => new()
    {
        cost = upgrade == Upgrade.A ? 0 : 1,
        artTint = "FFFFFF",
        exhaust = true
    };

    public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state)
    {
        if (upgrade == Upgrade.B)
        {
            return new HashSet<ICardTraitEntry>
            {
                ModEntry.Instance.Fleeting
            };
        }

        return new HashSet<ICardTraitEntry>();
    }
}