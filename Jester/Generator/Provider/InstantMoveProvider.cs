namespace Jester.Generator.Provider;

public class InstantMoveProvider : IProvider
{
    public List<IEntry> GetEntries(JesterRequest request)
    {
        var entries = new List<IEntry>();
        
        var minCost = request.MinCost;
        var maxCost = request.MaxCost;

        for (var i = 1; i <= 5; i++)
        {
            entries.Add(new InstantMoveEntry(this, i, false));
            entries.Add(new InstantMoveEntry(this, -i, false));
            entries.Add(new InstantMoveEntry(this, i, true));
        }

        return entries.Where(e => Util.InRange(minCost, e.GetCost(), maxCost)).ToList();
    }

    public class InstantMoveEntry : IEntry
    {
        private int Distance { get; }
        private bool Random { get; }

        public InstantMoveEntry(IProvider provider, int distance, bool random)
        {
            Provider = provider;
            Distance = distance;
            Random = random;
        }

        public HashSet<string> Tags
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
        public IProvider Provider { get; }
        public int GetActionCount() => 1;

        public List<CardAction> GetActions(State s, Combat c) => new()
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

        public IEntry GetUpgradeA(JesterRequest request, out int cost)
        {
            var entry = new InstantMoveEntry(Provider, Math.Sign(Distance) * (Math.Abs(Distance) + 1), Random);
            cost = entry.GetCost() - GetCost();
            return entry;
        }

        public IEntry? GetUpgradeB(JesterRequest request, out int cost)
        {
            if (!Random)
            {
                cost = 0;
                return null;
            }
            
            var entry = new InstantMoveEntry(Provider, Distance, false);
            cost = entry.GetCost() - GetCost();
            return entry;
        }

        public void AfterSelection(JesterRequest request)
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