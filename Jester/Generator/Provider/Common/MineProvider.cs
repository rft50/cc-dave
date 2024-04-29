using System.ComponentModel.DataAnnotations;
using Jester.Api;

namespace Jester.Generator.Provider.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class MineProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        var offsets = ModManifest.JesterApi.GetJesterUtil().GetDeployOptions(request.OccupiedMidrow);

        return offsets.Select(o => (1.0, new MineEntry
        {
            Offset = o,
            Upgraded = false,
            Shielded = false
        } as IEntry));
    }

    private class MineEntry : IEntry
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
                thing = new SpaceMine
                {
                    bigMine = Upgraded,
                    bubbleShield = Shielded,
                    targetPlayer = false
                },
                offset = Offset
            }
        };

        public int GetCost() => (Upgraded ? 30 : 20) + (Shielded ? 12 : 0);

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            var options = new List<(double, IEntry)>();
            if (!Upgraded)
                options.Add((1.0, new MineEntry
                {
                    Offset = Offset,
                    Shielded = Shielded,
                    Upgraded = true
                }));
            if (!Shielded)
                options.Add((0.5, new MineEntry
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
}