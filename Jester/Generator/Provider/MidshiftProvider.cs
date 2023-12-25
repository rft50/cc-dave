namespace Jester.Generator.Provider;

public class MidshiftProvider : IProvider
{
    public List<IEntry> GetEntries(JesterRequest request)
    {
        var entries = new List<IEntry>();
        
        var minCost = request.MinCost;
        var maxCost = request.MaxCost;

        for (var i = 1; i <= 5; i++)
        {
            entries.Add(new MidshiftEntry(i));
            entries.Add(new MidshiftEntry(-i));
        }

        return entries.Where(e => Util.InRange(minCost, e.GetCost(), maxCost)).ToList();
    }
    
    public class MidshiftEntry : IEntry
    {
        public int Distance { get; }

        public MidshiftEntry(int distance)
        {
            Distance = distance;
        }

        public HashSet<string> Tags =>
            new()
            {
                "utility",
                "midshift",
                "flippable"
            };
        public int GetActionCount() => 1;

        public List<CardAction> GetActions(State s, Combat c) => new()
        {
            new ADroneMove
            {
                dir = Distance
            }
        };

        public int GetCost()
        {
            return Distance * 7;
        }

        public IEntry GetUpgradeA(JesterRequest request, out int cost)
        {
            var entry = new MidshiftEntry(Math.Sign(Distance) * (Math.Abs(Distance) + 1));
            cost = entry.GetCost() - GetCost();
            return entry;
        }

        public IEntry? GetUpgradeB(JesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public void AfterSelection(JesterRequest request)
        {
            request.Blacklist.Add("midshift");
            request.OccupiedMidrow = request.OccupiedMidrow.Select(e => e + Distance).ToHashSet();
        }
    }
}