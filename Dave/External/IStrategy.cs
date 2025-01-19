namespace Dave.External;

public partial interface IJesterApi
{
    public void RegisterStrategy(IStrategy strategy);

    public IJesterRequest NewJesterRequest();
    
    public IJesterResult NewJesterResult();

    // Get all legal entries for the given costs
    public IEnumerable<(double, IEntry)> GetOptionsFromProviders(IJesterRequest request, IEnumerable<IProvider> providers);
    
    // Calls GetOptionsFromProviders and does a weighted random for you
    // Nullable in the event there are no legal entries
    public IEntry? GetRandomEntry(IJesterRequest request, IEnumerable<IProvider> providers, int actionCountLimit);
    
    // Attempt to spend as many points as possible on any available upgrades along the given path
    // Return value is the number of upgrades applied
    public int PerformUpgrade(IJesterRequest request, ref int pts, Upgrade upDir, int upgradeLimit = int.MaxValue);

    // Important if you are an Outer strategy trying to call an Inner strategy
    public IJesterResult CallInnerStrategy(IJesterRequest request, IEnumerable<IProvider> providers);
    
    public interface IStrategy
    {
        public IJesterResult GenerateCard(IJesterRequest request, IEnumerable<IProvider> providers);

        public double GetWeight(IJesterRequest request);

        public StrategyCategory GetStrategyCategory();
    }
    
    public interface IJesterResult
    {
        public IList<IEntry> Entries { get; set; }
        public CardData CardData { get; set; }
        public int SparePoints { get; set; }
    }
    
    public enum StrategyCategory
    {
        Full,
        Outer,
        Inner
    }
}


