using Jester.Api;

namespace Jester.Generator.Provider.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class OverdriveProvider : IProvider
{
    private static readonly int[] Costs = [25, 60];

    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        return Enumerable.Range(1, Costs.Length)
            .Select(i => (1.0/Costs.Length, new OverdriveEntry
            {
                Count = i
            } as IEntry));
    }

    private class OverdriveEntry : IEntry
    {
        public int Count { get; init; }
        
        public IReadOnlySet<string> Tags { get; } = new HashSet<string>
        {
            "offensive",
            "status",
            "overdrive"
        };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AStatus
            {
                status = Enum.Parse<Status>("overdrive"),
                statusAmount = Count,
                targetPlayer = true
            }
        };

        public int GetCost()
        {
            return Costs[Count - 1];
        }

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            if (Count >= Costs.Length) return new List<(double, IEntry)>();
            return new List<(double, IEntry)>
            {
                (1.0, new OverdriveEntry()
                {
                    Count = Count + 1
                })
            };
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("overdrive");
            request.Blacklist.Add("shot");
        }
    }
}