using System.Collections.Generic;

namespace InfiniteMode.Features.RestartOption;

public class CoreCorruption : IRestartOption
{
    public bool Selectable(State s) => true;
    public float GetWeight(State s) => 1;

    public List<CardAction> GetSingleActions(State s) =>
    [
        new AAddCard
        {
            card = new NonTempTrash(),
            destination = CardDestination.Deck,
            amount = 1
        }.WithDescription(ModEntry.Instance.Localizations.Localize(["restart", "option", "core", "one"]))
    ];

    public List<CardAction> GetDoubleActions(State s) =>
    [
        new AAddCard
        {
            card = new NonTempTrash(),
            destination = CardDestination.Deck,
            amount = 2
        }.WithDescription(ModEntry.Instance.Localizations.Localize(["restart", "option", "core", "two"]))
    ];

    public string GetSingleDescription(State s) => ModEntry.Instance.Localizations.Localize(["restart", "option", "core", "single"]);

    public string GetDoubleDescription(State s) => ModEntry.Instance.Localizations.Localize(["restart", "option", "core", "double"]);
}