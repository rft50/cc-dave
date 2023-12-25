namespace Jester.Generator.Provider;

public class StatusCostProvider : IProvider
{
    public List<IEntry> GetEntries(JesterRequest request)
    {
        if (!request.Whitelist.Contains("cost")) return new List<IEntry>();
        
        var minCost = request.MinCost;
        var maxCost = request.MaxCost;

        return new List<IEntry>
        {
            new StatusCostEntry(Enum.Parse<Status>("drawLessNextTurn"), 1, -15, "drawNext"),
            new StatusCostEntry(Enum.Parse<Status>("drawLessNextTurn"), 2, -15, "drawNext"),
            new StatusCostEntry(Enum.Parse<Status>("energyLessNextTurn"), 1, -20, "energyNext"),
            new StatusCostEntry(Enum.Parse<Status>("energyLessNextTurn"), 2, -20, "energyNext")
        }.Where(e => Util.InRange(minCost, e.GetCost(), maxCost))
        .ToList();
    }
    
    public class StatusCostEntry : IEntry
    {
        public Status Status { get; }
        public int Amount { get; }
        public int Cost { get; }
        public string Tag { get; }

        public StatusCostEntry(Status status, int amount, int cost, string tag)
        {
            Status = status;
            Amount = amount;
            Cost = cost;
            Tag = tag;
        }
        
        public HashSet<string> Tags => new()
        {
            "status",
            "cost",
            Tag
        };
        public int GetActionCount() => 1;

        public List<CardAction> GetActions(State s, Combat c) => new()
        {
            new AStatus
            {
                status = Status,
                statusAmount = Amount,
                targetPlayer = true
            }
        };

        public int GetCost() => Amount * Cost;

        public IEntry? GetUpgradeA(JesterRequest request, out int cost)
        {
            if (Amount <= 1)
            {
                cost = 0;
                return null;
            }

            cost = -Cost;
            return new StatusCostEntry(Status, Amount - 1, Cost, Tag);
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