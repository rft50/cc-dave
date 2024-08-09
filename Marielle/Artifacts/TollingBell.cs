using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Artifacts;

public class TollingBell : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("TollingBell", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = ModEntry.Instance.MarielleDeck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/TollingBell.png")).Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "TollingBell", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "TollingBell", "description"]).Localize
        });
    }

    public override int ModifyBaseDamage(int baseDamage, Card? card, State state, Combat? combat, bool fromPlayer)
    {
        return state.ship.Get(Status.heat) >= state.ship.heatTrigger
            ? state.ship.Get(ModEntry.Instance.Curse.Status)
            : 0;
    }
}