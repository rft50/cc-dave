using System.Collections.Generic;
using System.Reflection;
using Marielle.Cards;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Artifacts;

public class Repentance : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("Repentance", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = ModEntry.Instance.MarielleDeck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/repentance.png")).Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Repentance", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Repentance", "description"]).Localize
        });
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        combat.Queue(new AAddCard
        {
            card = new Absolve(),
            destination = CardDestination.Hand,
            artifactPulse = Key()
        });
    }

    public override List<Tooltip>? GetExtraTooltips()
    {
        return
        [
            new TTCard
            {
                card = new Absolve()
            }
        ];
    }
}