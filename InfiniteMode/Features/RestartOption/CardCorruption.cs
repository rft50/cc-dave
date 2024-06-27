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

    public string GetSingleDescription(State s) => "Corrupt 4 cards";

    public string GetDoubleDescription(State s) => "Corrupt 8 cards.";
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
    private string _text = "Corrupt Card";
    
    public override void Begin(G g, State s, Combat c)
    {
        var corrupted = CorruptedCardManager.Instance.GetCorruptedCards(s);
        var candidates = s.deck.Except(corrupted).ToList();
        if (candidates.Count == 0) return;

        var nonStarters = candidates.Where(card => !_startercards.Contains(card.GetType())).ToList();
        if (nonStarters.Count > 0)
            candidates = nonStarters;
        
        var card = candidates.ElementAt(s.rngActions.NextInt() % candidates.Count);
        corrupted.Add(card);
        CorruptedCardManager.Instance.SetCorruptedCards(s, corrupted);
        _text = "Corrupt " + card.Name();
    }

    public override string GetUpgradeText(State s) => _text;

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