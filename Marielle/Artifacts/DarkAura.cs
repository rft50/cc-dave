using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Artifacts;

public class DarkAura : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("DarkAura", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = ModEntry.Instance.MarielleDeck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/DarkAura.png")).Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "DarkAura", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "DarkAura", "description"]).Localize
        });
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        combat.Queue([
            new AStatus
            {
                status = ModEntry.Instance.Curse.Status,
                statusAmount = 1,
                targetPlayer = false,
                artifactPulse = Key()
            }
        ]);
    }
}