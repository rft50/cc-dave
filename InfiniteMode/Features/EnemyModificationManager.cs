using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using InfiniteMode.Artifacts;
using InfiniteMode.ExternalAPI;
using InfiniteMode.Features.DecisionModifier;
using InfiniteMode.Features.EnemyModifier;
using Microsoft.Extensions.Logging;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;

namespace InfiniteMode.Features;

[HarmonyPatch]
public class EnemyModificationManager
{
    [HarmonyPatch(typeof(Combat), "Make")]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Combat_Make_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase methodBase)
    {
        try
        {
            return new SequenceBlockMatcher<CodeInstruction>(instructions)
                .Find(
                    ILMatches.Stloc(typeof(Combat), methodBase).CreateLdlocInstruction(out var combat)
                )
                .Find(
                    ILMatches.Call("GetModifier"),
                    ILMatches.Stloc(typeof(FightModifier), methodBase).CreateLdlocInstruction(out var ldloc).CreateStlocInstruction(out var stloc)
                )
                .Insert(
                    SequenceMatcherPastBoundsDirection.After,
                    SequenceMatcherInsertionResultingBounds.JustInsertion,
                    new CodeInstruction(OpCodes.Ldarg_0),
                    combat.Value,
                    ldloc.Value,
                    new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(EnemyModificationManager), nameof(ModifyEnemySetup))),
                    stloc.Value
                )
                .AllElements();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    private static readonly List<IEnemyModifier> EnemyModifiers =
    [
        new HealthEnemyModifier(),
        new ShieldEnemyModifier()
    ];
    
    private static readonly List<IDecisionModifier> DecisionModifiers =
    [
        new AttackDecisionModifier(),
        
        // used by EnemyModifiers exclusively
        new ShieldDecisionModifier()
    ];
    
    private static FightModifier? ModifyEnemySetup(State s, Combat c, FightModifier? modifier)
    {
        var ptsCap = (s.EnumerateAllArtifacts().Find(a => a is InfinityArtifact) as InfinityArtifact)?.Level ?? 0;
        if (ptsCap == 0) return modifier;
        var shipPts = s.rngAi.NextInt() % ptsCap;
        var fightPts = ptsCap - shipPts;

        c.otherShip.hull = c.otherShip.hullMax += ptsCap * 2;

        var fightMod = PickFightModifier(s, c, modifier, ref shipPts);
        var enemyMods = new Dictionary<IEnemyModifier, int>();
        var decisionMods = new Dictionary<string, int>();

        while (shipPts > 0)
        {
            var pts = shipPts;
            WeightedRandom<IEnemyModifier> weightedRandom = new(
                EnemyModifiers.Where(e => e.GetCost(s, c) <= pts)
                    .Select(e => new WeightedItem<IEnemyModifier>(e.GetWeight(s, c), e))
            );
            var mod = weightedRandom.Next(s.rngAi);
            shipPts -= mod.GetCost(s, c);
            if (enemyMods.TryGetValue(mod, out var val))
                enemyMods[mod] = val + 1;
            else
                enemyMods[mod] = 1;
        }

        var decisionModsRaw = enemyMods.Keys
            .SelectMany(e =>
            {
                e.Apply(s, c, enemyMods[e], out var mods);
                return mods ?? [];
            });

        foreach (var decisionMod in decisionModsRaw)
        {
            var mod = decisionMod.ToString();
            if (decisionMods.TryGetValue(mod, out var val))
                decisionMods[mod] = val + 1;
            else
                decisionMods[mod] = 1;
        }
        
        ModEntry.Instance.Helper.ModData.SetModData(c, "fightPts", fightPts);
        ModEntry.Instance.Helper.ModData.SetModData(c, "decisionMods", decisionMods);

        return fightMod;
    }

    private static FightModifier? PickFightModifier(State s, Combat c, FightModifier? original, ref int pts)
    {
        return original;
    }
    
    [HarmonyPatch(typeof(AEnemyTurnAfter), "Begin")]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> AEnemyTurnAfter_Begin_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        try
        {
            return new SequenceBlockMatcher<CodeInstruction>(instructions)
                .Find(
                    ILMatches.Call("PickNextIntent")
                )
                .Insert(
                    SequenceMatcherPastBoundsDirection.After,
                    SequenceMatcherInsertionResultingBounds.JustInsertion,
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(EnemyModificationManager), nameof(ModifyEnemyDecision)))
                )
                .AllElements();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    private static EnemyDecision ModifyEnemyDecision(EnemyDecision decision, State s, Combat c)
    {
        if (!ModEntry.Instance.Helper.ModData.TryGetModData<Dictionary<string, int>>(c, "decisionMods",
                out var rawRequiredMods)
            || !ModEntry.Instance.Helper.ModData.TryGetModData<int>(c, "fightPts", out var fightPts))
            return decision;
        
        
        var decisionMods = new Dictionary<IDecisionModifier, int>();
        foreach (var (key, value) in rawRequiredMods!)
        {
            var decisionMod = DecisionModifiers.Find(e => e.GetType().ToString() == key);
            if (decisionMod != null)
                decisionMods[decisionMod] = value;
        }

        while (fightPts > 0)
        {
            var pts = fightPts;
            WeightedRandom<IDecisionModifier> weightedRandom = new(
                DecisionModifiers.Where(e => e.GetCost(s, c) <= pts)
                    .Select(e => new WeightedItem<IDecisionModifier>(e.GetWeight(s, c, decision), e))
            );
            var mod = weightedRandom.Next(s.rngAi);
            fightPts -= mod.GetCost(s, c);
            if (decisionMods.TryGetValue(mod, out var val))
                decisionMods[mod] = val + 1;
            else
                decisionMods[mod] = 1;
        }
        
        foreach (var (key, value) in decisionMods)
        {
            key.Apply(s, c, value, decision);
        }
        
        return decision;
    }
}