using System;
using System.Collections.Generic;
using System.Linq;
using Nickel;

namespace InfiniteMode.Features.RestartOption;

public class CardCorruption : IRestartOption
{
    public bool Selectable(State s) => s.deck.Count - CorruptedCardManager.Instance.GetCorruptedCards(s).Count >= 8;

    public float GetWeight(State s) => 1;

    public List<CardAction> GetSingleActions(State s) =>
    [
        new ACorruptCard(),
        new ACorruptCard(),
        new ACorruptCard(),
        new ACorruptCard()
    ];

    public List<CardAction> GetDoubleActions(State s) =>
    [
        new ACorruptCard(),
        new ACorruptCard(),
        new ACorruptCard(),
        new ACorruptCard(),
        new ACorruptCard(),
        new ACorruptCard(),
        new ACorruptCard(),
        new ACorruptCard()
    ];

    public string GetSingleDescription(State s) => ModEntry.Instance.Localizations.Localize(["restart", "option", "card", "single"]);

    public string GetDoubleDescription(State s) => ModEntry.Instance.Localizations.Localize(["restart", "option", "card", "double"]);
}

internal class ACorruptCard : CardAction
{
    private static List<Type> _startercards =
    [
        typeof(BasicShieldColorless),
        typeof(BasicSpacer),
        typeof(CannonColorless),
        typeof(DodgeColorless),
        typeof(DroneshiftColorless)
    ];
    public string Text = ModEntry.Instance.Localizations.Localize(["restart", "option", "card", "default"]);
    
    public override void Begin(G g, State s, Combat c)
    {
        var corrupted = CorruptedCardManager.Instance.GetCorruptedCards(s);
        var candidates = s.deck.Except(corrupted.Select(e => e.Item1)).ToList();
        if (candidates.Count == 0) return;

        var nonStarters = candidates.Where(card => !_startercards.Contains(card.GetType())).ToList();
        if (nonStarters.Count > 0)
            candidates = nonStarters;
        
        var card = candidates.ElementAt(s.rngActions.NextInt() % candidates.Count);
        CorruptedCardManager.Instance.CorruptCard(card, CorruptedCardManager.Instance.DetermineCorruptionLevel(card));
        Text = ModEntry.Instance.Localizations.Localize(["restart", "option", "card", "chosen"], new {Card = card.GetLocName()});
    }

    public override string GetUpgradeText(State s) => Text;

    public override List<Tooltip> GetTooltips(State s) =>
    [
        new GlossaryTooltip($"trait.{GetType().Namespace!}::CorruptedCard")
        {
            Icon = ModEntry.Instance.CorruptedCardTrait.Configuration.Icon(s, null),
            TitleColor = Colors.action,
            Title = ModEntry.Instance.Localizations.Localize(["trait", "CorruptedCard", "name"]),
            Description = ModEntry.Instance.Localizations.Localize(["trait", "CorruptedCard", "description"])
        }
    ];
}