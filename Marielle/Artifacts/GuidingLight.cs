using System.Collections.Generic;
using System.Reflection;
using Marielle.Cards;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Artifacts;

public class GuidingLight : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("GuidingLight", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = ModEntry.Instance.MarielleDeck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/GuidingLight.png")).Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "GuidingLight", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "GuidingLight", "description"]).Localize
        });
    }

    public override void OnReceiveArtifact(State state)
    {
        state.deck.Add(new Prayer { Discounted = true });
    }
    
    public override List<Tooltip>? GetExtraTooltips()
    {
        return
        [
            new TTCard
            {
                card = new Prayer { Discounted = true }
            }
        ];
    }
}