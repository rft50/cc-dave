namespace Jester.Generator.Provider;

public class AttackProvider : IProvider
{
    public List<IEntry> GetEntries(JesterRequest request)
    {
        var minCost = request.MinCost;
        var maxCost = request.MaxCost;
        var entries = new List<IEntry>();

        for (var i = 1; i <= 5; i++)
        {
            if (Util.InRange(minCost, 4 + i * 6, maxCost))
            {
                entries.Add(new AttackEntry(i, false));
            }
            if (Util.InRange(minCost, 4 + i * 10, maxCost))
            {
                entries.Add(new AttackEntry(i, true));
            }
        }

        if (Util.InRange(minCost, 10, maxCost))
        {
            entries.Add(new AttackEntry(1, false));
            entries.Add(new AttackEntry(1, false));
        }
        
        if (Util.InRange(minCost, 16, maxCost))
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

        public HashSet<string> Tags =>
            new()
            {
                "offensive",
                "attack",
                "shot"
            };
        public int GetActionCount() => 1;

        public List<CardAction> GetActions(State s, Combat c) => new()
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

        public IEntry GetUpgradeA(JesterRequest request, out int cost)
        {
            var entry = new AttackEntry(Damage + 1, Piercing);
            cost = entry.GetCost() - GetCost();
            return entry;
        }

        public IEntry? GetUpgradeB(JesterRequest request, out int cost)
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

        public void AfterSelection(JesterRequest request)
        {
        }

        public override string ToString()
        {
            return Piercing ? $"{Damage} Piercing" : $"{Damage} Damage";
        }
    }
}