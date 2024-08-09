using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Cards;

public class Absolve : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("Absolve", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.MarielleDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B],
                dontOffer = true
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Absolve", "name"]).Localize,
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/Absolve.png"))
                .Sprite
        });
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        var curse = ModEntry.Instance.Curse.Status;
        return upgrade switch
        {
            Upgrade.A =>
            [
                new AVariableHint
                {
                    status = curse
                },
                new AStatus
                {
                    status = Status.heat, statusAmount = -s.ship.Get(curse),
                    xHint = -1,
                    targetPlayer = true
                },
                new AStatus
                {
                    status = curse,
                    statusAmount = -4,
                    targetPlayer = true
                }
            ],
            Upgrade.B =>
            [
                new AVariableHint
                {
                    status = curse
                },
                new AStatus
                {
                    status = Status.heat,
                    statusAmount = -s.ship.Get(curse),
                    xHint = -1,
                    targetPlayer = true
                },
                new AStatus
                {
                    status = curse,
                    statusAmount = 0,
                    mode = AStatusMode.Set,
                    targetPlayer = true
                },
                new AStatus
                {
                    status = Status.serenity,
                    statusAmount = 1,
                    targetPlayer = true
                }
            ],
            _ =>
            [
                new AVariableHint
                {
                    status = curse
                },
                new AStatus
                {
                    status = Status.heat,
                    statusAmount = -s.ship.Get(curse),
                    xHint = -1,
                    targetPlayer = true
                },
                new AStatus
                {
                    status = curse,
                    statusAmount = 0,
                    mode = AStatusMode.Set,
                    targetPlayer = true
                }
            ]
        };
    }

    public override CardData GetData(State state) => new()
    {
        cost = 0,
        artTint = "FFFFFF",
        temporary = true,
        exhaust = true,
        retain = true
    };
}