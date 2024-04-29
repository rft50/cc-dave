using System.ComponentModel.DataAnnotations;
using Jester.Api;

namespace Jester.Generator.Provider.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class MissileProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        var offsets = ModManifest.JesterApi.GetJesterUtil().GetDeployOptions(request.OccupiedMidrow);

        return offsets.Select(o => (1.5, new MissileEntry
        {
            Offset = o,
            type = MissileType.normal
        } as IEntry));
    }

    private class MissileEntry : IEntry
    {
        private static Dictionary<MissileType, int> _costData = new()
        {
            {MissileType.normal, 15},
            {MissileType.heavy, 20},
            {MissileType.seeker, 18}
        };

        [Required] public MissileType type { get; init; }
        [Required] public int Offset { get; init; }

        public IReadOnlySet<string> Tags =>
            new HashSet<string>
            {
                "offensive",
                "missile",
                "attack"
            };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new ASpawn
            {
                thing = new Missile
                {
                    missileType = type,
                    targetPlayer = false
                },
                offset = Offset
            }
        };

        public int GetCost()
        {
            return _costData.GetValueOrDefault(type, 20);
        }

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            if (type != MissileType.normal) return new List<(double, IEntry)>();
            return new List<(double, IEntry)>
            {
                (0.75, new MissileEntry
                {
                    Offset = Offset,
                    type = MissileType.heavy
                }),
                (0.75, new MissileEntry
                {
                    Offset = Offset,
                    type = MissileType.seeker
                })
            };
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("shot");
            request.OccupiedMidrow.Add(Offset);
        }
    }
}