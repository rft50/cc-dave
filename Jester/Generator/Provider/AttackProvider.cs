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
                entries.Add(new AttackEntry(this, i, false));
            }
            if (Util.InRange(minCost, 4 + i * 10, maxCost))
            {
                entries.Add(new AttackEntry(this, i, true));
            }
        }

        if (Util.InRange(minCost, 10, maxCost))
        {
            entries.Add(new AttackEntry(this, 1, false));
            entries.Add(new AttackEntry(this, 1, false));
        }
        
        if (Util.InRange(minCost, 16, maxCost))
        {
            entries.Add(new AttackEntry(this, 2, false));
        }

        return entries;
    }

    public class AttackEntry : IEntry
    {
        private int Damage { get; }
        private bool Piercing { get; }

        public AttackEntry(IProvider provider, int damage, bool piercing)
        {
            Provider = provider;
            Damage = damage;
            Piercing = piercing;
        }

        public HashSet<string> Tags
        {
            get
            {
                if (Damage == 1 && !Piercing)
                    return new HashSet<string>
                    {
                        "starter",
                        "attack",
                        "shot"
                    };
                return new HashSet<string>
                {
                    "attack",
                    "shot"
                };
            }
        }

        public IProvider Provider { get; }
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
            var entry = new AttackEntry(Provider, Damage + 1, Piercing);
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
            var entry = new AttackEntry(Provider, Damage, true);
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