using System.ComponentModel.DataAnnotations;
using Jester.Api;

namespace Jester.Generator.Provider.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class EvadeProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        return Enumerable.Range(1, 3)
            .Select(i => (0.3, new EvadeEntry
            {
                Evade = i
            } as IEntry));
    }
    
    private class EvadeEntry : IEntry
    {
        [Required] public int Evade { get; init; }

        public IReadOnlySet<string> Tags =>
            new HashSet<string>
            {
                "defensive",
                "status",
                "evade",
                "move"
            };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AStatus
            {
                status = Enum.Parse<Status>("evade"),
                statusAmount = Evade,
                targetPlayer = true
            }
        };

        public int GetCost()
        {
            return Evade * 15;
        }

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            return new List<(double, IEntry)>
            {
                (1, new EvadeEntry
                {
                    Evade = Evade + 1
                })
            };
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("evade");
        }
    }
}