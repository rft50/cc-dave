using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Artifacts;

public class NoMercy : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("NoMercy", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = ModEntry.Instance.MarielleDeck.Deck,
                pools = [ArtifactPool.Boss]
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/NoMercy.png")).Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "NoMercy", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "NoMercy", "description"]).Localize
        });
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        combat.Queue([
            new AStatus
            {
                status = ModEntry.Instance.Curse.Status,
                statusAmount = 30,
                targetPlayer = false,
                artifactPulse = Key()
            },
        ]);
    }
}