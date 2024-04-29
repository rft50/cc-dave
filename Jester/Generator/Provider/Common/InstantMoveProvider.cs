using System.ComponentModel.DataAnnotations;
using Jester.Api;

namespace Jester.Generator.Provider.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class InstantMoveProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        if (request.Entries.Any(e => e.Tags.Contains("hermes"))) return new List<(double, IEntry)>();

        return Enumerable.Range(1, 5)
            .SelectMany(i => new List<(double, IEntry)>
            {
                (0.15, new InstantMoveEntry
                {
                    Distance = i,
                    Random = false
                }),
                (0.15, new InstantMoveEntry
                {
                    Distance = -i,
                    Random = false
                }),
                (0.15, new InstantMoveEntry
                {
                    Distance = i,
                    Random = true
                })
            });
    }

    private class InstantMoveEntry : IEntry
    {
        [Required] public int Distance { get; init; }
        [Required] public bool Random { get; init; }

        public IReadOnlySet<string> Tags
        {
            get
            {
                if (Random)
                    return new HashSet<string>
                    {
                        "defensive",
                        "move",
                        "random"
                    };
                return new HashSet<string>
                {
                    "defensive",
                    "move",
                    "flippable"
                };
            }
        }

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AMove
            {
                dir = Distance,
                isRandom = Random,
                targetPlayer = true
            }
        };

        public int GetCost()
        {
            if (Math.Abs(Distance) > 4 ) // big move: 5 for random, 6 for determined
                return Math.Abs(Distance) * (Random ? 5 : 6);
            if (Random) // rando move: lerps from 4 to 5 per dist
                return Distance * (15 + Distance) / 4;
            if (Distance > 0) // right move: 6 per dist
                return Distance * 6;
            // left move: lerps from 4 to 6 per dist
            return Distance * (7 + Distance) / 2;
        }
        
        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            var options = new List<(double, IEntry)>
            {
                (1, new InstantMoveEntry
                {
                    Distance = Math.Sign(Distance) * (Math.Abs(Distance) + 1),
                    Random = Random
                })
            };
            if (Random)
                options.Add((1, new InstantMoveEntry
                {
                    Distance = Distance,
                    Random = false
                }));
            return options;
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("move");
            if (Random)
            {
                var result = request.OccupiedMidrow.Select(e => e - Distance).ToHashSet();
                result.UnionWith(request.OccupiedMidrow.Select(e => e + Distance).ToHashSet());
                request.OccupiedMidrow = result;
            }
            else
            {
                request.OccupiedMidrow = request.OccupiedMidrow.Select(e => e - Distance).ToHashSet();
            }
        }

        public override string ToString()
        {
            if (Random)
                return $"{Distance} Move Random";
            return Distance > 0 ? $"{Distance} Move Right" : $"{Distance} Move Left";
        }
    }
}