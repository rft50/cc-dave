using Jester.Api;

namespace Jester.Generator.Provider;

public class EvadeProvider : IProvider
{
    public IList<IEntry> GetEntries(IJesterRequest request)
    {
        var entries = new List<IEntry>();

        var minCost = request.MinCost;
        var maxCost = request.MaxCost;

        for (var i = 1; i <= 3; i++)
        {
            if (ModManifest.JesterApi.GetJesterUtil().InRange(minCost, i * 10, maxCost))
            {
                entries.Add(new EvadeEntry(i));
            }
        }

        return entries;
    }
    
    public class EvadeEntry : IEntry
    {
        private int Evade { get; }

        public EvadeEntry(int evade)
        {
            Evade = evade;
        }


        public ISet<string> Tags => new HashSet<string>
            {
                "defensive",
                "status",
                "evade",
                "move"
            };
        public int GetActionCount() => 1;

        public IList<CardAction> GetActions(State s, Combat c) => new List<CardAction>
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
            return Evade * 10;
        }

        public IEntry GetUpgradeA(IJesterRequest request, out int cost)
        {
            cost = 10;
            return new EvadeEntry(Evade + 1);
        }

        public IEntry? GetUpgradeB(IJesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("evade");
        }
    }
}