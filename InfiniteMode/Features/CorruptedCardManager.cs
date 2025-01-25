using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Nickel;

namespace InfiniteMode.Features;

[HarmonyPatch]
public class CorruptedCardManager
{
    public static readonly CorruptedCardManager Instance = new();
    
    public void Register()
    {
        ModEntry.Instance.Helper.Content.Cards.OnGetDynamicInnateCardTraitOverrides += (_, data) =>
        {
            if (GetCardCorruption(data.Card) != null)
            {
                data.SetOverride(ModEntry.Instance.CorruptedCardTrait, true);
            }
        };
        ModEntry.Instance.Helper.Events.RegisterBeforeArtifactsHook("OnCombatStart", (Combat combat, State state) =>
        {
            foreach (var (card, uses) in GetCorruptedCards(state)
                         .Where(e => e.Item2 >= 0))
            {
                ModEntry.Instance.KokoroApi.Limited.SetLimitedUses(state, card, uses);
            }
        });
    }
    
    public List<(Card, int)> GetCorruptedCards(State s)
    {
        return s.deck.Select(c =>
                (c, ModEntry.Instance.Helper.ModData.TryGetModData<int>(c, "corruption", out var d) ? d : -1))
            .Where(c => c.Item2 >= 0)
            .ToList();
    }

    public int? GetCardCorruption(Card c)
    {
        return ModEntry.Instance.Helper.ModData.TryGetModData<int>(c, "corruption", out var d) ? d : null;
    }
    
    // Positive: Limited amount
    // Zero: Single Use
    // null: Remove Corruption
    public void CorruptCard(Card card, int? corruptionLevel)
    {
        if (corruptionLevel == null)
            ModEntry.Instance.Helper.ModData.RemoveModData(card, "corruption");
        else
            ModEntry.Instance.Helper.ModData.SetModData(card, "corruption", corruptionLevel);
    }

    public int DetermineCorruptionLevel(Card card)
    {
        var s = State.FakeState();
        if (ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(s, card,
                ModEntry.Instance.Helper.Content.Cards.SingleUseCardTrait))
            return 0;
        if (ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(s, card,
                ModEntry.Instance.Helper.Content.Cards.ExhaustCardTrait))
            return 1;
        if (ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(s, card,
                ModEntry.Instance.KokoroApi.Limited.Trait))
            return ModEntry.Instance.KokoroApi.Limited.GetStartingLimitedUses(s, card);
        return 4;
    }

    [HarmonyPatch(typeof(Combat), "PlayerWon")]
    [HarmonyPostfix]
    private static void Combat_PlayerWon_Postfix(G g)
    {
        if (Instance.GetCorruptedCards(g.state).Count == 0) return;
        Util.ApplyToShipUpgrades(g.state, [
            new ATickCorruption(),
            new ADummyAction()
        ]);
    }
}

public class ATickCorruption : CardAction
{
    public string Description = ModEntry.Instance.Localizations.Localize(["corruption", "card", "tick"]);
    
    public override void Begin(G g, State s, Combat c)
    {
        var corrupted = CorruptedCardManager.Instance.GetCorruptedCards(s);
        if (corrupted.Count == 0) return;
        var (card, corruption) = corrupted[s.rngScript.NextInt() % corrupted.Count];
        if (corruption == 0)
        {
            s.deck.Remove(card);
            Description = ModEntry.Instance.Localizations.Localize(["corruption", "card", "destroy"], new {Name = card.GetFullDisplayName()});
        }
        else if (corruption == 1)
        {
            ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(s, card, ModEntry.Instance.KokoroApi.Limited.Trait, false, true);
            ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(s, card, ModEntry.Instance.Helper.Content.Cards.ExhaustCardTrait, false, true);
            ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(s, card, ModEntry.Instance.Helper.Content.Cards.SingleUseCardTrait, true, true);
            CorruptedCardManager.Instance.CorruptCard(card, corruption - 1);
            Description = ModEntry.Instance.Localizations.Localize(["corruption", "card", "singleUse"], new {Name = card.GetFullDisplayName()});
        }
        else
        {
            ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(s, card, ModEntry.Instance.KokoroApi.Limited.Trait, true, true);
            CorruptedCardManager.Instance.CorruptCard(card, corruption - 1);
            Description = ModEntry.Instance.Localizations.Localize(["corruption", "card", "limited"], new {Name = card.GetFullDisplayName(), Amount = corruption - 1});
        }
    }

    public override string GetUpgradeText(State s)
    {
        return Description;
    }
}