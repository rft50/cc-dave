using Jester.Generator.Provider;

namespace Jester.Generator;

public interface IEntry
{
    public HashSet<string> Tags { get; }
    
    public IProvider Provider { get; }

    public int GetActionCount();

    public List<CardAction> GetActions(State s, Combat c);

    public int GetCost();

    public IEntry? GetUpgradeA(JesterRequest request, out int cost);

    public IEntry? GetUpgradeB(JesterRequest request, out int cost);

    public void AfterSelection(JesterRequest request);
}