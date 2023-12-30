using Jester.Api;

namespace Jester.Generator.Provider;

public class DroneshiftProvider : IProvider
{
    public IList<IEntry> GetEntries(IJesterRequest request)
    {
        var entries = new List<IEntry>();

        var minCost = request.MinCost;
        var maxCost = request.MaxCost;

        for (var i = 1; i <= 3; i++)
        {
            if (ModManifest.JesterApi.GetJesterUtil().InRange(minCost, i * 10, maxCost))
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


        public ISet<string> Tags => new HashSet<string>
            {
                "defensive",
                "status",
                "droneshift",
                "move"
            };
        public int GetActionCount() => 1;

        public IList<CardAction> GetActions(State s, Combat c) => new List<CardAction>
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

        public IEntry GetUpgradeA(IJesterRequest request, out int cost)
        {
            cost = 8;
            return new DroneshiftEntry(Droneshift + 1);
        }

        public IEntry? GetUpgradeB(IJesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("droneshift");
        }
    }
}