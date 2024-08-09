using System;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Artifacts;

public class HotPotato : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("HotPotato", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = ModEntry.Instance.MarielleDeck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/HotPotato.png")).Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "HotPotato", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "HotPotato", "description"]).Localize
        });
    }

    public override void AfterPlayerStatusAction(State state, Combat combat, Status status, AStatusMode mode, int statusAmount)
    {
        if (status != Status.heat) return;
        if (mode == AStatusMode.Add && statusAmount <= 0) return;
        if (mode == AStatusMode.Set && statusAmount <= state.ship.Get(Status.heat)) return;
        if (mode == AStatusMode.Mult && Math.Sign(statusAmount) * Math.Sign(state.ship.Get(Status.heat)) <= 0) return;
        
        combat.Queue(new AStatus
        {
            status = Status.heat,
            statusAmount = 1,
            targetPlayer = false,
            artifactPulse = Key()
        });
    }
}