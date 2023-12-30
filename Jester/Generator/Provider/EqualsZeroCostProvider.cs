using Jester.Api;

namespace Jester.Generator.Provider;

public class EqualsZeroCostProvider : IProvider
{
    public IList<IEntry> GetEntries(IJesterRequest request)
    {
        if (!request.Whitelist.Contains("cost")) return new List<IEntry>();
        
        var minCost = request.MinCost;
        var maxCost = request.MaxCost;

        return new List<IEntry>
        {
            new EqualsZeroCostEntry(Enum.Parse<Status>("shield"), "shield", 30),
            new EqualsZeroCostEntry(Enum.Parse<Status>("evade"), "evade", 30)
        }.Where(e => ModManifest.JesterApi.GetJesterUtil().InRange(minCost, e.GetCost(), maxCost))
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

        public ISet<string> Tags => new HashSet<string>
        {
            "cost",
            Tag
        };

        public int GetActionCount() => 1;

        public IList<CardAction> GetActions(State s, Combat c) => new List<CardAction>
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

        public IEntry? GetUpgradeA(IJesterRequest request, out int cost)
        {
            cost = 0;
            return null;
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