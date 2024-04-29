using Jester.Api;

namespace Jester.Generator.Provider.Drake;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class LessHeatProvider : IProvider
{
    private static int[] _costs = { 7, 15, 24 };

    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        if (!ModManifest.JesterApi.HasCharacterFlag("heat")) return new List<(double, IEntry)>();
        return Enumerable.Range(1, _costs.Length)
            .Select(i => (4.0/_costs.Length, new LessHeatEntry
            {
                Count = i
            } as IEntry));
    }

    private class LessHeatEntry : IEntry
    {
        public int Count { get; set; }
        
        public IReadOnlySet<string> Tags { get; } = new HashSet<string>
        {
            "status",
            "heat"
        };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AStatus
            {
                status = Enum.Parse<Status>("heat"),
                statusAmount = -Count,
                targetPlayer = true
            }
        };

        public int GetCost()
        {
            return _costs[Count - 1];
        }

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            if (Count >= _costs.Length) return new List<(double, IEntry)>();
            return new List<(double, IEntry)>
            {
                (1.0, new LessHeatEntry
                {
                    Count = Count + 1
                })
            };
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("heat");
        }
    }
}