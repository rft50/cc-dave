namespace Jester.Generator.Provider;

public class EvadeProvider : IProvider
{
    public List<IEntry> GetEntries(JesterRequest request)
    {
        var entries = new List<IEntry>();

        var minCost = request.MinCost;
        var maxCost = request.MaxCost;

        for (var i = 1; i <= 3; i++)
        {
            if (Util.InRange(minCost, i * 10, maxCost))
            {
                entries.Add(new EvadeEntry(this, i));
            }
        }

        return entries;
    }
    
    public class EvadeEntry : IEntry
    {
        private int Evade { get; }

        public EvadeEntry(IProvider provider, int evade)
        {
            Provider = provider;
            Evade = evade;
        }


        public HashSet<string> Tags =>
            new()
            {
                "defensive",
                "evade",
                "move"
            };

        public IProvider Provider { get; }
        public int GetActionCount() => 1;

        public List<CardAction> GetActions(State s, Combat c) => new()
        {
            new AStatus
            {
                status = Enum.Parse<Status>("evade"),
                statusAmount = Evade,
                targetPlayer = true
            }
        };

        public int GetCost()
        {
            return Evade * 10;
        }

        public IEntry GetUpgradeA(JesterRequest request, out int cost)
        {
            cost = 10;
            return new EvadeEntry(Provider, Evade + 1);
        }

        public IEntry? GetUpgradeB(JesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public void AfterSelection(JesterRequest request)
        {
            request.Blacklist.Add("evade");
        }
    }
}