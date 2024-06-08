using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Cards;

public class WickedPact : Card, IRegisterable, IHasCustomCardTraits
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("WickedPact", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.MarielleDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "WickedPact", "name"]).Localize,
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/WickedPact.png")).Sprite
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
                    status = Status.evade,
                    statusAmount = 2,
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
        return
        [
            new AStatus
            {
                status = Status.evade,
                statusAmount = 2,
                targetPlayer = true
            },
            new AStatus
            {
                status = ModEntry.Instance.Curse.Status,
                statusAmount = 1,
                targetPlayer = true
            },
            new AStatus
            {
                status = Status.heat,
                statusAmount = upgrade == Upgrade.A ? 1 : 2,
                targetPlayer = true
            }
        ];
    }

    public override CardData GetData(State state) => new()
    {
        cost = 1
    };

    public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state)
    {
        if (upgrade == Upgrade.B)
            return new HashSet<ICardTraitEntry>
            {
                ModEntry.Instance.Fleeting
            };
        
        return new HashSet<ICardTraitEntry>();
    }
}