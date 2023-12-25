namespace Jester.Generator.Provider;

public class EqualsZeroCostProvider : IProvider
{
    public List<IEntry> GetEntries(JesterRequest request)
    {
        if (!request.Whitelist.Contains("cost")) return new List<IEntry>();
        
        var minCost = request.MinCost;
        var maxCost = request.MaxCost;

        return new List<IEntry>
        {
            new EqualsZeroCostEntry(Enum.Parse<Status>("shield"), "shield", 30),
            new EqualsZeroCostEntry(Enum.Parse<Status>("evade"), "evade", 30)
        }.Where(e => Util.InRange(minCost, e.GetCost(), maxCost))
        .ToList();
    }
    
    public class EqualsZeroCostEntry : IEntry
    {
        public Status Status { get; }
        public string Tag { get; }
        public int Cost { get; }

        public EqualsZeroCostEntry(Status status, string tag, int cost)
        {
            Status = status;
            Tag = tag;
            Cost = cost;
        }

        public HashSet<string> Tags => new()
        {
            "cost",
            Tag
        };

        public int GetActionCount() => 1;

        public List<CardAction> GetActions(State s, Combat c) => new()
        {
            new AStatus
            {
                status = Status,
                statusAmount = 0,
                mode = AStatusMode.Set,
                targetPlayer = true
            }
        };

        public int GetCost() => Cost;

        public IEntry? GetUpgradeA(JesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public IEntry? GetUpgradeB(JesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public void AfterSelection(JesterRequest request)
        {
            request.Blacklist.Add(Tag);
        }
    }
}