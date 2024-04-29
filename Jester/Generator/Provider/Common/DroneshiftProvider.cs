using System.ComponentModel.DataAnnotations;
using Jester.Api;

namespace Jester.Generator.Provider.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class DroneshiftProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        return Enumerable.Range(1, 3)
            .Select(i => (0.3, new DroneshiftEntry
            {
                Droneshift = i
            } as IEntry));
    }
    
    private class DroneshiftEntry : IEntry
    {
        [Required] public int Droneshift { get; init; }
        
        public IReadOnlySet<string> Tags =>
            new HashSet<string>
            {
                "defensive",
                "status",
                "droneshift",
                "move"
            };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
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

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            return new List<(double, IEntry)>
            {
                (1, new DroneshiftEntry
                {
                    Droneshift = Droneshift + 1
                })
            };
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("droneshift");
        }
    }
}