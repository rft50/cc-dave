namespace Jester.Generator.Provider;

public class DroneshiftProvider : IProvider
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
                entries.Add(new DroneshiftEntry(i));
            }
        }

        return entries;
    }
    
    public class DroneshiftEntry : IEntry
    {
        private int Droneshift { get; }

        public DroneshiftEntry(int droneshift)
        {
            Droneshift = droneshift;
        }


        public HashSet<string> Tags =>
            new()
            {
                "defensive",
                "status",
                "droneshift",
                "move"
            };
        public int GetActionCount() => 1;

        public List<CardAction> GetActions(State s, Combat c) => new()
        {
            new AStatus
            {
                status = Enum.Parse<Status>("droneShift"),
                statusAmount = Droneshift,
                targetPlayer = true
            }
        };

        public int GetCost()
        {
            return Droneshift * 8;
        }

        public IEntry GetUpgradeA(JesterRequest request, out int cost)
        {
            cost = 8;
            return new DroneshiftEntry(Droneshift + 1);
        }

        public IEntry? GetUpgradeB(JesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public void AfterSelection(JesterRequest request)
        {
            request.Blacklist.Add("droneshift");
        }
    }
}