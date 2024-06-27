using System.Collections.Generic;

namespace InfiniteMode.Features.RestartOption;

public interface IRestartOption
{
    public bool Selectable(State s);
    public float GetWeight(State s);
    public List<CardAction> GetSingleActions(State s);
    public List<CardAction> GetDoubleActions(State s);
    public string GetSingleDescription(State s);
    public string GetDoubleDescription(State s);
}