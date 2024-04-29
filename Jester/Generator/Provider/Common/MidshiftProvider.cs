using System.ComponentModel.DataAnnotations;
using Jester.Api;

namespace Jester.Generator.Provider.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class MidshiftProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        return Enumerable.Range(1, 5)
            .SelectMany(i => new List<(double, IEntry)>
            {
                (0.1, new MidshiftEntry
                {
                    Distance = i
                }),
                (0.1, new MidshiftEntry
                {
                    Distance = -i
                })
            });
    }
    
    private class MidshiftEntry : IEntry
    {
        [Required] public int Distance { get; init; }

        public IReadOnlySet<string> Tags =>
            new HashSet<string>
            {
                "utility",
                "midshift",
                "flippable"
            };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new ADroneMove
            {
                dir = Distance
            }
        };

        public int GetCost()
        {
            return Math.Abs(Distance) * 7;
        }

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            return new List<(double, IEntry)>
            {
                (1, new MidshiftEntry
                {
                    Distance = Math.Sign(Distance) * (Math.Abs(Distance) + 1)
                }),
            };
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("midshift");
            request.OccupiedMidrow = request.OccupiedMidrow.Select(e => e + Distance).ToHashSet();
        }
    }
}