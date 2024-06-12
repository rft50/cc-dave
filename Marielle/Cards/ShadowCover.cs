using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Cards;

public class ShadowCover : Card, IRegisterable, IHasCustomCardTraits
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("ShadowCover", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.MarielleDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ShadowCover", "name"]).Localize,
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/ShadowCover.png")).Sprite
        });
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return
        [
            new AStatus
            {
                status = Status.shield,
                statusAmount = upgrade == Upgrade.A ? 3 : 2,
                targetPlayer = true
            },
            new AStatus
            {
                status = ModEntry.Instance.Curse.Status,
                statusAmount = 1,
                targetPlayer = true
            }
        ];
    }

    public override CardData GetData(State state) => new()
    {
        cost = 1,
        artTint = "FFFFFF"
    };

    public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state)
    {
        if (upgrade == Upgrade.B)
            return new HashSet<ICardTraitEntry>();
        return new HashSet<ICardTraitEntry>
        {
            ModEntry.Instance.Fleeting
        };
    }
}