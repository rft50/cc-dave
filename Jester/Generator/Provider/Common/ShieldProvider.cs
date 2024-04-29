using System.ComponentModel.DataAnnotations;
using Jester.Api;

namespace Jester.Generator.Provider.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class ShieldProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        return Enumerable.Range(1, 3)
            .SelectMany(i => new List<(double, IEntry)>
            {
                (0.3, new ShieldEntry
                {
                    Shield = i,
                    Temp = true
                }),
                (0.3, new ShieldEntry
                {
                    Shield = i,
                    Temp = false
                })
            });
    }

    class ShieldEntry : IEntry
    {
        [Required] public int Shield { get; init; }
        [Required] public bool Temp { get; init; }
        
        public IReadOnlySet<string> Tags
        {
            get
            {
                if (!Temp)
                    return new HashSet<string>
                    {
                        "defensive",
                        "shield"
                    };
                return new HashSet<string>
                {
                    "defensive",
                    "tempShield"
                };
            }
        }

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AStatus
            {
                status = Enum.Parse<Status>(Temp ? "tempShield" : "shield"),
                statusAmount = Shield,
                targetPlayer = true
            }
        };

        public int GetCost()
        {
            return Shield * (Temp ? 8 : 10);
        }
        
        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            var options = new List<(double, IEntry)>
            {
                (1, new ShieldEntry
                {
                    Shield = Shield + 1,
                    Temp = Temp
                })
            };
            if (Temp && !request.Entries.Any(e => e is ShieldEntry))
                options.Add((1, new ShieldEntry
                    {
                        Shield = Shield,
                        Temp = false
                    }));
            return options;
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add(Temp ? "tempShield" : "shield");
        }
    }
}