using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Nanoray.PluginManager;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Nickel;

namespace Marielle.Artifacts;

[HarmonyPatch]
public class ColdHearted : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("ColdHearted", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = ModEntry.Instance.MarielleDeck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/ColdHearted.png")).Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ColdHearted", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ColdHearted", "description"]).Localize
        });
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        state.ship.heatMin -= 2;
        state.ship.Add(Status.heat, -2);
        state.ship.heatMin += 2;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Ship), "Set")]
    private static IEnumerable<CodeInstruction> Ship_Set_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        try
        {
            return new SequenceBlockMatcher<CodeInstruction>(instructions)
                .Find(
                    ILMatches.Ldfld("heatMin")
                )
                .Insert(
                    SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.JustInsertion,
                    [
                        new(OpCodes.Ldarg_0),
                        new(OpCodes.Ldc_I4, (int) Status.heat),
                        new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(Ship), "Get")),
                        new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(Math), "Min", [typeof(int), typeof(int)]))
                    ]
                )
                .AllElements();
        }
        catch (Exception e)
        {
            Console.WriteLine("ColdHearted's Ship.Set patch failed!");
            Console.WriteLine(e);
            return instructions;
        }
    }
}