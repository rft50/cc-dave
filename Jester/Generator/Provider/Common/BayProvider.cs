using Jester.Api;

namespace Jester.Generator.Provider.Common;

public class BayProvider : IProvider
{
    public IList<IEntry> GetEntries(IJesterRequest request)
    {
        var minCost = request.MinCost;
        var maxCost = request.MaxCost;

        var offsets = ModManifest.JesterApi.GetJesterUtil().GetDeployOptions(request.OccupiedMidrow);
        var entries = new List<BayEntry>();
        
        foreach (var offset in offsets)
        {
            entries.Add(new BayEntry(new AttackDrone(),
                new HashSet<string>
                {
                    "offensive",
                    "drone",
                    "attack"
                }, 20, offset, true));
            entries.Add(new BayEntry(new AttackDrone
                {
                    upgraded = true
                },
                new HashSet<string>
                {
                    "offensive",
                    "drone",
                    "attack"
                }, 35, offset, true));
            entries.Add(new BayEntry(new ShieldDrone
                {
                    targetPlayer = true
                },
                new HashSet<string>
                {
                    "defensive",
                    "drone"
                }, 18, offset, true));
            entries.Add(new BayEntry(new EnergyDrone
                {
                    targetPlayer = true
                },
                new HashSet<string>
                {
                    "utility",
                    "drone"
                }, 50, offset, true));
            entries.Add(new BayEntry(new Missile
                {
                    missileType = Enum.Parse<MissileType>("normal")
                },
                new HashSet<string>
                {
                    "offensive",
                    "missile",
                    "attack"
                }, 15, offset, false));
            entries.Add(new BayEntry(new Missile
                {
                    missileType = Enum.Parse<MissileType>("heavy")
                },
                new HashSet<string>
                {
                    "offensive",
                    "missile",
                    "attack"
                }, 20, offset, false));
            entries.Add(new BayEntry(new Missile
                {
                    missileType = Enum.Parse<MissileType>("seeker")
                },
                new HashSet<string>
                {
                    "offensive",
                    "missile",
                    "attack"
                }, 18, offset, false));
            entries.Add(new BayEntry(new Missile
                {
                    missileType = Enum.Parse<MissileType>("corrode")
                },
                new HashSet<string>
                {
                    "offensive",
                    "missile",
                    "attack"
                }, 60, offset, false));
            entries.Add(new BayEntry(new SpaceMine(),
                new HashSet<string>
                {
                    "offensive",
                    "attack"
                }, 20, offset, true));
            entries.Add(new BayEntry(new SpaceMine
                {
                    bigMine = true
                },
                new HashSet<string>
                {
                    "offensive",
                    "attack"
                }, 30, offset, true));
        }
        
        entries.AddRange(entries.Where(e => e.Shieldable)
            .Select(e =>
            {
                var clone = Mutil.DeepCopy(e.Payload);
                return new BayEntry(clone, e.Tags, e.Cost + 12, e.Offset, true);
            }).ToList());
        
        return entries.Where(e => ModManifest.JesterApi.GetJesterUtil().InRange(minCost, e.GetCost(), maxCost)).ToList<IEntry>();
    }

    public class BayEntry : IEntry
    {
        public StuffBase Payload { get; set; } = null!;
        public int Cost { get; set; }
        public int Offset { get; set; }
        public bool Shieldable { get; set; }

        public BayEntry()
        {
            
        }

        public BayEntry(StuffBase payload, ISet<string> tags, int cost, int offset, bool shieldable)
        {
            Payload = payload;
            Tags = tags;
            Cost = cost;
            Offset = offset;
            Shieldable = shieldable;

            tags.Add("bay");
            if (offset != 0)
                tags.Add("flippable");
        }
        
        public ISet<string> Tags { get; } = null!;
        public int GetActionCount() => 1;

        public IList<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new ASpawn
            {
                thing = Mutil.DeepCopy(Payload),
                offset = Offset
            }
        };

        public int GetCost() => Cost;

        public IEntry? GetUpgradeA(IJesterRequest request, out int cost)
        {
            if (Payload is not AttackDrone drone || drone.upgraded) return GetUpgradeB(request, out cost);
            
            cost = 15;
                
            var clone = Mutil.DeepCopy(drone);
            clone.upgraded = true;
            return new BayEntry(clone, Tags, Cost + cost, Offset, Shieldable);
        }

        public IEntry? GetUpgradeB(IJesterRequest request, out int cost)
        {
            if (Shieldable && !Payload.bubbleShield)
            {
                cost = 12;
                
                var clone = Mutil.DeepCopy(Payload);
                clone.bubbleShield = true;
                return new BayEntry(clone, Tags, Cost + cost, Offset, true);
            }

            cost = 0;
            return null;
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("shot");
            request.OccupiedMidrow.Add(Offset);
        }

        public override string? ToString()
        {
            return Payload.ToString();
        }
    }
}