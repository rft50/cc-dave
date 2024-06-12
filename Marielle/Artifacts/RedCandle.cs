using System.Linq;
using System.Reflection;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Artifacts;

[HarmonyPatch]
public class RedCandle : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("RedCandle", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = ModEntry.Instance.MarielleDeck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/RedCandle.png")).Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "RedCandle", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "RedCandle", "description"]).Localize
        });
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AStatus), "Begin")]
    private static void AStatus_Postfix(AStatus __instance, State s, Combat c)
    {
        if (__instance.targetPlayer || !s.EnumerateAllArtifacts().Any(a => a is RedCandle)) return;
        if (c.otherShip.Get(Status.heat) >= c.otherShip.heatTrigger)
        {
            c.QueueImmediate(new AOverheat
            {
                targetPlayer = false
            });
        }
    }
}