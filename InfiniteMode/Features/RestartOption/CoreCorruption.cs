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
        }.WithDescription("+ 1 Debris")
    ];

    public List<CardAction> GetDoubleActions(State s) =>
    [
        new AAddCard
        {
            card = new NonTempTrash(),
            destination = CardDestination.Deck,
            amount = 2
        }.WithDescription("+ 2 Debris")
    ];

    public string GetSingleDescription(State s) => "Add 1 Debris to your deck";

    public string GetDoubleDescription(State s) => "Add 2 Debris to your deck.";
}