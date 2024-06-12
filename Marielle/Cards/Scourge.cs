using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Cards;

public class Scourge : Card, IRegisterable, IHasCustomCardTraits
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("Scourge", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.MarielleDeck.Deck,
                rarity = Rarity.rare,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Scourge", "name"]).Localize,
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/Scourge.png"))
                .Sprite
        });
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return
        [
            new AStatus
            {
                status = Status.tempShield,
                statusAmount = upgrade == Upgrade.A ? 2 : 1,
                targetPlayer = true
            },
            new AStatus
            {
                status = ModEntry.Instance.Curse.Status,
                statusAmount = 1,
                targetPlayer = false
            }
        ];
    }

    public override CardData GetData(State state) => new()
    {
        cost = 1,
        artTint = "FFFFFF",
        infinite = upgrade != Upgrade.B,
        recycle = upgrade == Upgrade.B
    };

    public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state)
    {
        return new HashSet<ICardTraitEntry>
        {
            ModEntry.Instance.Fleeting
        };
    }
}