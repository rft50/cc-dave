using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Nickel;

namespace InfiniteMode.Features;

[HarmonyPatch]
public class CorruptedCardManager
{
    public static readonly CorruptedCardManager Instance = new();
    
    public void Register(IModHelper helper)
    {
        helper.Content.Cards.OnGetVolatileCardTraitOverrides += (_, data) =>
        {
            var s = data.State;
            if (GetCorruptedCards(s).Contains(data.Card))
            {
                data.SetVolatileOverride(ModEntry.Instance.CorruptedCardTrait, true);
            }
        };
    }
    
    public List<Card> GetCorruptedCards(State s)
    {
        if (!ModEntry.Instance.KokoroApi.TryGetExtensionData<List<int>>(s, "corruptedCards", out var data)) return [];
        
        return data.Select(s.FindCard)
            .Where(card => card != null)
            .Cast<Card>()
            .ToList();
    }
    
    public void SetCorruptedCards(State s, List<Card> cards)
    {
        ModEntry.Instance.KokoroApi.SetExtensionData(s, "corruptedCards", cards
            .Select(c => c.uuid)
            .ToList()
        );
    }

    [HarmonyPatch(typeof(Combat), "PlayerWon")]
    [HarmonyPostfix]
    private static void Combat_PlayerWon_Postfix(G g)
    {
        var s = g.state;
        var corrupted = Instance.GetCorruptedCards(s);
        if (corrupted.Count == 0) return;
        var card = corrupted[s.rngScript.NextInt() % corrupted.Count];
        s.deck.Remove(card);
    }
}