using Jester.Api;

namespace Jester.Generator.Provider.Common;

public class StatusCostProvider : IProvider
{
    public IList<IEntry> GetEntries(IJesterRequest request)
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
        }.Where(e => ModManifest.JesterApi.GetJesterUtil().InRange(minCost, e.GetCost(), maxCost))
        .ToList();
    }
    
    public class StatusCostEntry : IEntry
    {
        public Status Status { get; set; }
        public int Amount { get; set; }
        public int Cost { get; set; }
        public string Tag { get; set; } = null!;

        public StatusCostEntry()
        {
        }
        public StatusCostEntry(Status status, int amount, int cost, string tag)
        {
            Status = status;
            Amount = amount;
            Cost = cost;
            Tag = tag;
        }
        
        public ISet<string> Tags
        {
            get => new HashSet<string>
            {
                "status",
                "cost",
                Tag
            };
            
        }

        public int GetActionCount() => 1;

        public IList<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AStatus
            {
                status = Status,
                statusAmount = Amount,
                targetPlayer = true
            }
        };

        public int GetCost() => Amount * Cost;

        public IEntry? GetUpgradeA(IJesterRequest request, out int cost)
        {
            if (Amount <= 1)
            {
                cost = 0;
                return null;
            }

            cost = -Cost;
            return new StatusCostEntry(Status, Amount - 1, Cost, Tag);
        }

        public IEntry? GetUpgradeB(IJesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add(Tag);
        }
    }
}