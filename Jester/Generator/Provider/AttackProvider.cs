using Jester.Api;

namespace Jester.Generator.Provider;

public class AttackProvider : IProvider
{
    public IList<IEntry> GetEntries(IJesterRequest request)
    {
        var minCost = request.MinCost;
        var maxCost = request.MaxCost;
        var entries = new List<IEntry>();

        for (var i = 1; i <= 5; i++)
        {
            if (ModManifest.JesterApi.GetJesterUtil().InRange(minCost, 4 + i * 6, maxCost))
            {
                entries.Add(new AttackEntry(i, false));
            }
            if (ModManifest.JesterApi.GetJesterUtil().InRange(minCost, 4 + i * 10, maxCost))
            {
                entries.Add(new AttackEntry(i, true));
            }
        }

        if (ModManifest.JesterApi.GetJesterUtil().InRange(minCost, 10, maxCost))
        {
            entries.Add(new AttackEntry(1, false));
            entries.Add(new AttackEntry(1, false));
        }
        
        if (ModManifest.JesterApi.GetJesterUtil().InRange(minCost, 16, maxCost))
        {
            entries.Add(new AttackEntry(2, false));
        }

        return entries;
    }

    public class AttackEntry : IEntry
    {
        private int Damage { get; }
        private bool Piercing { get; }

        public AttackEntry(int damage, bool piercing)
        {
            Damage = damage;
            Piercing = piercing;
        }

        public ISet<string> Tags => new HashSet<string>
            {
                "offensive",
                "attack",
                "shot"
            };
        public int GetActionCount() => 1;

        public IList<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AAttack
            {
                damage = Card.GetActualDamage(s, Damage),
                piercing = Piercing
            }
        };

        public int GetCost()
        {
            return 4 + Damage * (6 + (Piercing ? 4 : 0));
        }

        public IEntry GetUpgradeA(IJesterRequest request, out int cost)
        {
            var entry = new AttackEntry(Damage + 1, Piercing);
            cost = entry.GetCost() - GetCost();
            return entry;
        }

        public IEntry? GetUpgradeB(IJesterRequest request, out int cost)
        {
            if (Piercing)
            {
                cost = 0;
                return null;
            }
            var entry = new AttackEntry(Damage, true);
            cost = entry.GetCost() - GetCost();
            return entry;
        }

        public void AfterSelection(IJesterRequest request)
        {
        }

        public override string ToString()
        {
            return Piercing ? $"{Damage} Piercing" : $"{Damage} Damage";
        }
    }
}