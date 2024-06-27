using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace InfiniteMode.Artifacts;

public class InfinityArtifact : Artifact, IRegisterable
{
    public int Level = 1;
    
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("InfinityArtifact", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = Deck.colorless,
                pools = [ArtifactPool.EventOnly]
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/InfinityArtifact.png")).Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "InfinityArtifact", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "InfinityArtifact", "description"])
                .Localize
        });
    }

    public override int? GetDisplayNumber(State s) => Level;
}