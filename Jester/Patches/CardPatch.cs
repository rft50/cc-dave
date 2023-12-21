using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Jester.Artifacts;
using Jester.Cards;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;

namespace Jester.Patches;

[HarmonyPatch(typeof(Card))]
public class CardPatch
{
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Card.Render))]
    private static IEnumerable<CodeInstruction> RenderTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator ilGenerator, MethodBase originalMethod)
    {
        try
        {
            return new SequenceBlockMatcher<CodeInstruction>(instructions)
                .Find(
                    ILMatches.Ldloc<CardData>(originalMethod.GetMethodBody()!.LocalVariables),
                    ILMatches.Ldfld("buoyant"),
                    ILMatches.Brfalse
                )
                .PointerMatcher(SequenceMatcherRelativeElement.First)
                .ExtractLabels(out var labels)
                .Anchors().AnchorPointer(out var findAnchor)
                .Find(
                    ILMatches.Ldloc<Vec>(originalMethod.GetMethodBody()!.LocalVariables),
                    ILMatches.Ldfld("y"),
                    ILMatches.LdcI4(8),
                    ILMatches.Ldloc<int>(originalMethod.GetMethodBody()!.LocalVariables),
                    ILMatches.Instruction(OpCodes.Dup),
                    ILMatches.LdcI4(1),
                    ILMatches.Instruction(OpCodes.Add),
                    ILMatches.Stloc<int>(originalMethod.GetMethodBody()!.LocalVariables)
                )
                .PointerMatcher(SequenceMatcherRelativeElement.First)
                .CreateLdlocInstruction(out var ldlocVec)
                .Advance(3)
                .CreateLdlocaInstruction(out var ldlocaCardTraitIndex)
                .Anchors().PointerMatcher(findAnchor)
                .Insert(
                    SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
                    new CodeInstruction(OpCodes.Ldarg_1).WithLabels(labels),
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    ldlocaCardTraitIndex,
                    ldlocVec,
                    new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(CardPatch), nameof(RenderJesterCardTraits)))
                )
                .AllElements();
        }
        catch (Exception e)
        {
            Console.WriteLine("Card.Render patch failed!");
            Console.WriteLine(e);
            return instructions;
        }
    }

    private static void RenderJesterCardTraits(G g, State? state, Card card, ref int cardTraitIndex, Vec vec)
    {
        state ??= g.state;
        var scripted = GetOpeningScriptedCards(state);

        if (scripted.Contains(card.uuid) && ModManifest.OpeningScriptedGlossary != null)
            Draw.Sprite((Spr)ModManifest.OpeningScriptedGlossary.Icon.Id!, vec.x, vec.y - 8 * cardTraitIndex++);
        if (card is AbstractJoker && ModManifest.JesterRandomGlossary != null)
            Draw.Sprite((Spr)ModManifest.JesterRandomGlossary.Icon.Id!, vec.x, vec.y - 8 * cardTraitIndex++);
    }

    private static List<int> GetOpeningScriptedCards(State state)
    {
        var script = (OpeningScript?)state.EnumerateAllArtifacts().Find(a => a is OpeningScript);
        return script == null ? new List<int>() : script.CardData.Select(d => d.Item1).ToList();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Card.GetAllTooltips))]
    private static void GetAllTooltipsPostfix(Card __instance, State s, bool showCardTraits,
        ref IEnumerable<Tooltip> __result)
    {
        if (!showCardTraits) return;
        
        var needShowScripted = GetOpeningScriptedCards(s).Contains(__instance.uuid);
        var needShowRandom = __instance is AbstractJoker;
        
        if (!needShowScripted && !needShowRandom) return;

        IEnumerable<Tooltip> ModifyTooltips(IEnumerable<Tooltip> tooltips)
        {
            foreach (var tooltip in tooltips)
            {
                if (tooltip is TTGlossary glossary)
                {
                    if (needShowScripted && glossary.key.StartsWith("cardtrait.") &&
                        glossary.key != "cardtrait.unplayable")
                    {
                        needShowScripted = false;
                        yield return ModManifest.OpeningScriptedTooltip;
                    }
                    if (needShowRandom && glossary.key.StartsWith("cardtrait.") &&
                        glossary.key != "cardtrait.unplayable")
                    {
                        needShowRandom = false;
                        yield return ModManifest.JesterRandomTooltip;
                    }
                }
                
                if (needShowScripted)
                    yield return ModManifest.OpeningScriptedTooltip;
                if (needShowRandom)
                    yield return ModManifest.JesterRandomTooltip;

                yield return tooltip;
            }
        }

        __result = ModifyTooltips(__result);
    }
}