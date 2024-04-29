using System.ComponentModel.DataAnnotations;
using Jester.Api;

namespace Jester.Generator.Provider.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class EqualsZeroCostProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        if (!request.Whitelist.Contains("cost")) return new List<(double, IEntry)>();

        return new List<(double, IEntry)>
        {
            (1, new EqualsZeroCostEntry
            {
                Status = Enum.Parse<Status>("shield"),
                Tag = "shield",
                Cost = -30
            }),
            (1, new EqualsZeroCostEntry
            {
                Status = Enum.Parse<Status>("evade"),
                Tag = "evade",
                Cost = -30
            })
        };
    }
    
    private class EqualsZeroCostEntry : IEntry
    {
        [Required] public Status Status { get; init; }
        [Required] public string Tag { get; set; } = null!;
        [Required] public int Cost { get; init; }

        public IReadOnlySet<string> Tags =>
            new HashSet<string>
            {
                "cost",
                Tag
            };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
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

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            return new List<(double, IEntry)>();
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add(Tag);
        }
    }
}