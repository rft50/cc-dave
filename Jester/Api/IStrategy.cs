using Jester.Generator.Provider;

namespace Jester.Api;

public partial interface IJesterApi
{
    public void RegisterStrategy(IStrategy strategy);

    public IJesterRequest NewJesterRequest();
    
    public IJesterResult NewJesterResult();

    // Get all legal entries for the given costs
    public IList<IEntry> GetOptionsFromProviders(IJesterRequest request, IEnumerable<IProvider> providers);

    // Get all legal entries for the given costs and blacklist/whitelist
    public IList<IEntry> GetOptionsFromProvidersFiltered(IJesterRequest request, IEnumerable<IProvider> providers);

    // Attempt to spend as many points as possible on the given entries
    // Return value is total points spent
    public int PerformUpgradeA(IJesterRequest request, IList<IEntry> entries, ref int pts);

    // Important if you are an Outer strategy trying to call an Inner strategy
    public IJesterResult CallInnerStrategy(IJesterRequest request, IList<IProvider> providers, int maxActions);
}

public interface IStrategy
{
    public IJesterResult GenerateCard(IJesterRequest request, IList<IProvider> providers, int maxActions);

    public double GetWeight(IJesterRequest request);

    public StrategyCategory GetStrategyCategory();
}

public interface IJesterResult
{
    public IList<IEntry> Entries { get; set; }
    public CardData CardData { get; set; }
}

public enum StrategyCategory
{
    Full,
    Outer,
    Inner
}