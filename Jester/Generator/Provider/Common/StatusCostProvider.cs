using System.ComponentModel.DataAnnotations;
using Jester.Api;

namespace Jester.Generator.Provider.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class StatusCostProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        if (!request.Whitelist.Contains("cost")) return new List<(double, IEntry)>();

        return Enumerable.Range(1, 2)
            .SelectMany(i => new List<(double, IEntry)>
            {
                (0.5, new StatusCostEntry
                {
                    Status = Enum.Parse<Status>("drawLessNextTurn"),
                    Amount = i,
                    CostPer = -15,
                    Tag = "drawNext"
                }),
                (0.5, new StatusCostEntry
                {
                    Status = Enum.Parse<Status>("energyLessNextTurn"),
                    Amount = i,
                    CostPer = -20,
                    Tag = "energyNext"
                })
            });
    }
    
    private class StatusCostEntry : IEntry
    {
        [Required] public Status Status { get; init; }
        [Required] public int Amount { get; init; }
        [Required] public int CostPer { get; init; }
        [Required] public string Tag { get; set; } = null!;
        
        public IReadOnlySet<string> Tags =>
            new HashSet<string>
            {
                "status",
                "cost",
                Tag
            };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AStatus
            {
                status = Status,
                statusAmount = Amount,
                targetPlayer = true
            }
        };

        public int GetCost() => Amount * CostPer;

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            if (Amount <= 1) return new List<(double, IEntry)>();
            return new List<(double, IEntry)>
            {
                (1, new StatusCostEntry
                {
                    Status = Status,
                    Amount = Amount - 1,
                    CostPer = CostPer,
                    Tag = Tag
                })
            };
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add(Tag);
        }
    }
}