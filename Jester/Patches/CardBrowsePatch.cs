using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;

namespace Jester.Patches;

[HarmonyPatch(typeof(CardBrowse))]
public class CardBrowsePatch
{
    public static List<Card>? CardsToInject { get; set; }
    
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

    private static void InjectCards(List<Card> cardList)
    {
        if (CardsToInject == null) return;
        cardList.Clear();
        cardList.AddRange(CardsToInject);
    }

    [HarmonyPostfix]
    [HarmonyPatch("OnPickCardAction")]
    private static void OnPickCardAction()
    {
        CardsToInject = null;
    }
}