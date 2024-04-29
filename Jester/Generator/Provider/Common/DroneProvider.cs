using System.ComponentModel.DataAnnotations;
using Jester.Api;

namespace Jester.Generator.Provider.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class DroneProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        var offsets = ModManifest.JesterApi.GetJesterUtil().GetDeployOptions(request.OccupiedMidrow);

        return offsets.SelectMany(o => new List<(double, IEntry)>
        {
            (0.65, new AttackDroneEntry
            {
                Offset = o,
                Shielded = false,
                Upgraded = false
            }),
            (0.35, new AttackDroneEntry
            {
                Offset = o,
                Shielded = false,
                Upgraded = true
            }),
            (1.0, new ShieldDroneEntry
            {
                Offset = o,
                Shielded = false
            }),
            (1.0, new EnergyDroneEntry
            {
                Offset = o,
                Shielded = false
            })
        });
    }

    private class AttackDroneEntry : IEntry
    {
        [Required] public int Offset { get; init; }
        [Required] public bool Shielded { get; init; }
        [Required] public bool Upgraded { get; init; }

        public IReadOnlySet<string> Tags =>
            new HashSet<string>
            {
                "offensive",
                "drone",
                "attack"
            };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new ASpawn
            {
                thing = new AttackDrone
                {
                    upgraded = Upgraded,
                    bubbleShield = Shielded,
                    targetPlayer = false
                },
                offset = Offset
            }
        };

        public int GetCost() => (Upgraded ? 35 : 20) + (Shielded ? 12 : 0);

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            var options = new List<(double, IEntry)>();
            if (!Upgraded)
                options.Add((1.0, new AttackDroneEntry
                {
                    Offset = Offset,
                    Shielded = Shielded,
                    Upgraded = true
                }));
            if (!Shielded)
                options.Add((1.0, new AttackDroneEntry
                {
                    Offset = Offset,
                    Shielded = true,
                    Upgraded = Upgraded
                }));
            return options;
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("shot");
            request.OccupiedMidrow.Add(Offset);
        }
    }

    private class ShieldDroneEntry : IEntry
    {
        [Required] public int Offset { get; init; }
        [Required] public bool Shielded { get; init; }

        public IReadOnlySet<string> Tags =>
            new HashSet<string>
            {
                "defensive",
                "drone"
            };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new ASpawn
            {
                thing = new ShieldDrone
                {
                    bubbleShield = Shielded,
                    targetPlayer = true
                },
                offset = Offset
            }
        };

        public int GetCost() => 18 + (Shielded ? 12 : 0);

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            var options = new List<(double, IEntry)>();
            if (!Shielded)
                options.Add((1.0, new ShieldDroneEntry
                {
                    Offset = Offset,
                    Shielded = true
                }));
            return options;
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("shot");
            request.OccupiedMidrow.Add(Offset);
        }
    }

    private class EnergyDroneEntry : IEntry
    {
        [Required] public int Offset { get; init; }
        [Required] public bool Shielded { get; init; }

        public IReadOnlySet<string> Tags =>
            new HashSet<string>
            {
                "utility",
                "drone"
            };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new ASpawn
            {
                thing = new EnergyDrone
                {
                    bubbleShield = Shielded,
                    targetPlayer = true
                },
                offset = Offset
            }
        };

        public int GetCost() => 50 + (Shielded ? 12 : 0);

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            var options = new List<(double, IEntry)>();
            if (!Shielded)
                options.Add((1.0, new EnergyDroneEntry
                {
                    Offset = Offset,
                    Shielded = true
                }));
            return options;
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("shot");
            request.OccupiedMidrow.Add(Offset);
        }
    }
}