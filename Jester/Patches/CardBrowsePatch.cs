using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Jester.Actions;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;

namespace Jester.Patches;

[HarmonyPatch(typeof(CardBrowse))]
public class CardBrowsePatch
{
    [HarmonyTranspiler]
    [HarmonyPatch("GetCardList")]
    private static IEnumerable<CodeInstruction> GetCardListTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator il, MethodBase originalMethod)
    {
        try
        {
            var localVars = originalMethod.GetMethodBody()!.LocalVariables;

            return new SequenceBlockMatcher<CodeInstruction>(instructions)
                .Find(
                    ILMatches.Ldloc<List<Card>>(localVars),
                    ILMatches.AnyLdloc,
                    ILMatches.Call("AddRange")
                )
                .PointerMatcher(SequenceMatcherRelativeElement.First)
                .CreateLdlocInstruction(out var cardList)
                .Advance(2)
                .Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.JustInsertion,
                    new List<CodeInstruction>
                    {
                        new(OpCodes.Ldarg_0),
                        cardList,
                        new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(CardBrowsePatch), nameof(InjectCards)))
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

    private static void InjectCards(CardBrowse browse, List<Card> cardList)
    {
        if (browse is not ArbitraryCardBrowse acb) return;
        var toInject = acb.Cards;
        
        if (toInject == null) return;
        cardList.Clear();
        cardList.AddRange(toInject);
    }
}