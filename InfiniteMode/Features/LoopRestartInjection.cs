using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using InfiniteMode.Artifacts;
using InfiniteMode.Map;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;

namespace InfiniteMode.Features;

[HarmonyPatch]
public class LoopRestartInjection
{
    [HarmonyPatch(typeof(MapBase), "Populate")]
    [HarmonyPostfix]
    private static void MapBase_Populate_Postfix(MapBase __instance)
    {
        if (__instance is not MapThree) return;
        
        var markers = __instance.markers;
        var furthest = markers
            .Select(v => v.Key.y)
            .Max();
        var freeY = markers
            .Where(v => Math.Abs(v.Key.y - furthest) < 0.01)
            .Select(v => v.Key.x)
            .Min();
        markers.Add(new Vec(freeY + 1, furthest + 1), new Marker
        {
            contents = new MapLoopRestart()
        });
        markers[new Vec(freeY, furthest)].paths.Add((int) freeY + 1);
    }

    [HarmonyPatch(typeof(Combat), "PlayerWon")]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Combat_PlayerWon_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
    {
        try
        {
            return new SequenceBlockMatcher<CodeInstruction>(instructions)
                .Find(
                    ILMatches.Br,
                    ILMatches.LdcI4(0),
                    ILMatches.Stloc<bool>(originalMethod).CreateLdlocInstruction(out var ldloc).CreateStlocInstruction(out var stloc)
                )
                .Insert(
                    SequenceMatcherPastBoundsDirection.After,
                    SequenceMatcherInsertionResultingBounds.JustInsertion,
                    ldloc.Value,
                    new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(LoopRestartInjection), nameof(EndRun))),
                    stloc.Value
                )
                .AllElements();
        }
        catch (Exception e)
        {
            throw;
        }
    }
    
    private static bool EndRun(bool game) // game is what the game says
    {
        return false;
    }

    [HarmonyPatch(typeof(Combat), "PlayerWon")]
    [HarmonyPostfix]
    private static void Combat_PlayerWon_Postfix(G g, Combat __instance)
    {
        var s = g.state;
        if (s.map.markers[s.map.currentLocation].contents is not MapBattle contents) return;
        if (contents.battleType != BattleType.Boss) return;

        if (s.EnumerateAllArtifacts().Find(v => v is InfinityArtifact) is InfinityArtifact infinity)
        {
            infinity.Level++;
            infinity.Pulse();
        }

        CorruptedArtifactStatus? status = null;
        if (s.map.GetType() == typeof(MapFirst))
            status = CorruptedArtifactStatus.Zone1;
        else if (s.map.GetType() == typeof(MapLawless))
            status = CorruptedArtifactStatus.Zone2;
        if (status == null) return;
        
        var artifacts = s.EnumerateAllArtifacts()
            .Where(a =>
               CorruptedArtifactManager.Instance.GetArtifactCorruption(a) == status)
            .ToList();
        Util.ApplyToShipUpgrades(g.state, artifacts
            .Select(a => new ALoseArtifact
            {
                artifactType = a.Key()
            }.WithDescription(ModEntry.Instance.Localizations.Localize(["corruption", "artifact", "lose"], new {Name = a.GetLocName()}))));
    }
}