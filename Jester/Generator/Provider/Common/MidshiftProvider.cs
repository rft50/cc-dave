using Jester.Api;

namespace Jester.Generator.Provider.Common;

public class MidshiftProvider : IProvider
{
    public IList<IEntry> GetEntries(IJesterRequest request)
    {
        var entries = new List<IEntry>();
        
        var minCost = request.MinCost;
        var maxCost = request.MaxCost;

        for (var i = 1; i <= 5; i++)
        {
            entries.Add(new MidshiftEntry(i));
            entries.Add(new MidshiftEntry(-i));
        }

        return entries.Where(e => ModManifest.JesterApi.GetJesterUtil().InRange(minCost, e.GetCost(), maxCost)).ToList();
    }
    
    public class MidshiftEntry : IEntry
    {
        public int Distance { get; set; }

        public MidshiftEntry()
        {
        }
        public MidshiftEntry(int distance)
        {
            Distance = distance;
        }

        public ISet<string> Tags
        {
            get => new HashSet<string>
            {
                "utility",
                "midshift",
                "flippable"
            };
            
        }

        public int GetActionCount() => 1;

        public IList<CardAction> GetActions(State s, Combat c) => new List<CardAction>
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

        public IEntry GetUpgradeA(IJesterRequest request, out int cost)
        {
            var entry = new MidshiftEntry(Math.Sign(Distance) * (Math.Abs(Distance) + 1));
            cost = entry.GetCost() - GetCost();
            return entry;
        }

        public IEntry? GetUpgradeB(IJesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("midshift");
            request.OccupiedMidrow = request.OccupiedMidrow.Select(e => e + Distance).ToHashSet();
        }
    }
}