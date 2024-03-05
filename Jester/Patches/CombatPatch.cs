using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;

namespace Jester.Patches;

[HarmonyPatch(typeof(Combat))]
public class CombatPatch
{
    public static bool DoCardCheck = true;
    public static bool DoCardDisposal = true;
    
    [HarmonyPostfix]
    [HarmonyPatch("TryPlayCard")]
    public static void TryPlayCardPostfix(Card card, State s, Combat __instance, bool __result)
    {
        if (__result)
            CardPlayTracker.RegisterCardPlay(card, s, __instance);
        
        DoCardCheck = true;
        DoCardDisposal = true;
    }

    [HarmonyTranspiler]
    [HarmonyPatch("TryPlayCard")]
    private static IEnumerable<CodeInstruction> TryPlayCardTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator il, MethodBase originalMethod)
    {
        try
        {
            var localVars = originalMethod.GetMethodBody()!.LocalVariables;

            return new SequenceBlockMatcher<CodeInstruction>(instructions)
                .Find(
                    ILMatches.Ldarg(0),
                    ILMatches.Ldfld("hand"),
                    ILMatches.AnyLdloc,
                    ILMatches.Ldfld("card"),
                    ILMatches.AnyCall,
                    ILMatches.Brtrue
                )
                .PointerMatcher(SequenceMatcherRelativeElement.Last)
                .GetBranchTarget(out var ifLabel)
                .Advance(-5)
                .Insert(SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.JustInsertion,
                    new List<CodeInstruction>
                    {
                        new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(CombatPatch), nameof(BypassCardCheck))),
                        new(OpCodes.Brtrue, ifLabel)
                    })
                
                .Find(
                    ILMatches.Ldarg(0),
                    ILMatches.Ldfld("hand"),
                    ILMatches.AnyLdloc,
                    ILMatches.Ldfld("card"),
                    ILMatches.Call("Remove"),
                    ILMatches.Instruction(OpCodes.Pop)
                )
                .PointerMatcher(SequenceMatcherRelativeElement.AfterLast)
                .CreateLabel(il, out var pastRemoveCard)
                .Find(SequenceBlockMatcherFindOccurence.First, SequenceMatcherRelativeBounds.WholeSequence,
                    ILMatches.Ldarg(0),
                    ILMatches.Ldfld("hand"),
                    ILMatches.Call("Count"),
                    ILMatches.AnyStloc
                )
                .Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.JustInsertion,
                    new List<CodeInstruction>
                    {
                        new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(CombatPatch), nameof(BypassCardDisposal))),
                        new(OpCodes.Brtrue, pastRemoveCard)
                    }
                )
                
                .Find(
                    ILMatches.Ldarg(0).ExtractLabels(out var labels),
                    ILMatches.Ldfld("hand"),
                    ILMatches.AnyLdloc,
                    ILMatches.Ldfld("card"),
                    ILMatches.Call("Remove"),
                    ILMatches.Instruction(OpCodes.Pop)
                )
                .Replace(
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(labels),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(CombatPatch), nameof(RemoveCard)))
                )
                
                .Find(
                    ILMatches.Ldarg(0),
                    ILMatches.Ldfld("hand"),
                    ILMatches.Call("GetEnumerator"),
                    ILMatches.AnyStloc
                )
                .PointerMatcher(SequenceMatcherRelativeElement.First)
                .CreateLabel(il, out var endOfIf)
                .Find(SequenceBlockMatcherFindOccurence.Last, SequenceMatcherRelativeBounds.WholeSequence,
                    ILMatches.Ldloc<CardData>(localVars),
                    ILMatches.Ldfld("singleUse"),
                    ILMatches.Brfalse
                )
                .PointerMatcher(SequenceMatcherRelativeElement.First)
                .Insert(SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.JustInsertion,
                    new List<CodeInstruction>
                    {
                        new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(CombatPatch), nameof(BypassCardDisposal))),
                        new(OpCodes.Brtrue, endOfIf)
                    })
                .AllElements();
        }
        catch (Exception e)
        {
            Console.WriteLine("CardBrowse.GetCardList patch failed!");
            Console.WriteLine(e);
            return instructions;
        }
    }

    private static bool BypassCardCheck() => !DoCardCheck;

    private static bool BypassCardDisposal() => !DoCardDisposal;

    private static void RemoveCard(Combat c, State s, Card card)
    {
        s.deck.Remove(card);
        c.hand.Remove(card);
        c.discard.Remove(card);
        c.exhausted.Remove(card);
    }
}