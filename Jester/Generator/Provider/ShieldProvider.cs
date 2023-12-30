using Jester.Api;

namespace Jester.Generator.Provider;

public class ShieldProvider : IProvider
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
                entries.Add(new ShieldEntry(i, false));
            }
            if (ModManifest.JesterApi.GetJesterUtil().InRange(minCost, i * 8, maxCost))
            {
                entries.Add(new ShieldEntry(i, true));
            }
        }

        return entries;
    }

    class ShieldEntry : IEntry
    {
        public int Shield { get; }
        public bool Temp { get; }

        public ShieldEntry(int shield, bool temp)
        {
            Shield = shield;
            Temp = temp;
        }
        
        public ISet<string> Tags { 
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
            } }
        public int GetActionCount() => 1;

        public IList<CardAction> GetActions(State s, Combat c) => new List<CardAction>
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

        public IEntry GetUpgradeA(IJesterRequest request, out int cost)
        {
            var entry = new ShieldEntry(Shield + 1, Temp);
            cost = entry.GetCost() - GetCost();
            return entry;
        }

        public IEntry? GetUpgradeB(IJesterRequest request, out int cost)
        {
            if (request.Entries.Any(e => e is ShieldEntry))
            {
                cost = 0;
                return null;
            }
            
            var entry = new ShieldEntry(Shield, false);
            cost = entry.GetCost() - GetCost();
            return entry;
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add(Temp ? "tempShield" : "shield");
        }
    }
}