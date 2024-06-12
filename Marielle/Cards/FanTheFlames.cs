using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Cards;

public class FanTheFlames : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("FanTheFlames", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = ModEntry.Instance.MarielleDeck.Deck,
                rarity = Rarity.rare,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "FanTheFlames", "name"]).Localize,
            Art = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/FanTheFlames.png")).Sprite
        });
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        return upgrade switch
        {
            Upgrade.A =>
            [
                ModEntry.Instance.KokoroApi.Actions.SetTargetPlayer(new AVariableHint
                {
                    status = Status.heat
                }, false),
                new AStatus
                {
                    status = Status.heat, statusAmount = c.otherShip.Get(Status.heat),
                    xHint = 1,
                    targetPlayer = false
                },
                new AStatus
                {
                    status = Status.heat, statusAmount = 1, targetPlayer = false
                }
            ],
            Upgrade.B =>
            [
                new AVariableHint
                {
                    status = Status.heat
                },
                new AStatus
                {
                    status = Status.heat, statusAmount = s.ship.Get(Status.heat),
                    xHint = 1,
                    targetPlayer = false
                }
            ],
            _ =>
            [
                ModEntry.Instance.KokoroApi.Actions.SetTargetPlayer(new AVariableHint
                {
                    status = Status.heat
                }, false),
                new AStatus
                {
                    status = Status.heat, statusAmount = c.otherShip.Get(Status.heat),
                    xHint = 1,
                    targetPlayer = false
                }
            ]
        };
    }

    public override CardData GetData(State state) => new()
    {
        cost = 1,
        artTint = "FFFFFF"
    };
}