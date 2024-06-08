using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Cards;

public class Brimstone : Card, IRegisterable, IHasCustomCardTraits
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("Brimstone", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.MarielleDeck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Brimstone", "name"]).Localize,
            Art = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/Brimstone.png")).Sprite
        });
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return
        [
            new AStatus
            {
                status = Status.tempShield,
                statusAmount = upgrade == Upgrade.A ? 4 : 2,
                targetPlayer = true
            },
            new AStatus
            {
                status = ModEntry.Instance.Enflamed.Status,
                statusAmount = 1,
                targetPlayer = false
            }
        ];
    }

    public override CardData GetData(State state) => new()
    {
        cost = 2
    };

    public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state)
    {
        if (upgrade == Upgrade.B)
        {
            return new HashSet<ICardTraitEntry>();
        }

        return new HashSet<ICardTraitEntry>
        {
            ModEntry.Instance.Fleeting
        };
    }
}