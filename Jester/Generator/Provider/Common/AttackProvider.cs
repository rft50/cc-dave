using Jester.Api;

namespace Jester.Generator.Provider.Common;

public class AttackProvider : IProvider
{
    public IList<IEntry> GetEntries(IJesterRequest request)
    {
        var minCost = request.MinCost;
        var maxCost = request.MaxCost;
        var entries = new List<IEntry>();
        var existingShotCount = request.Entries
            .Count(e => e.Tags.Contains("shot"));

        for (var i = 1; i <= 5; i++)
        {
            entries.Add(new AttackEntry(i, false, existingShotCount));
            entries.Add(new AttackEntry(i, true, existingShotCount));
        }

        return entries
            .Where(e => ModManifest.JesterApi.GetJesterUtil().InRange(minCost, e.GetCost(), maxCost))
            .ToList();
    }

    public class AttackEntry : IEntry
    {
        public int Damage { get; set; }
        public bool Piercing { get; set; }
        public int ExistingShotCount { get; set; }
        
        public AttackEntry()
        {
        }

        public AttackEntry(int damage, bool piercing, int existingShotCount)
        {
            Damage = damage;
            Piercing = piercing;
            ExistingShotCount = existingShotCount;
        }

        public ISet<string> Tags
        {
            get => new HashSet<string>
            {
                "offensive",
                "attack",
                "shot"
            };
            
        }

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
            return Damage * (Piercing ? 13 : 10) + ExistingShotCount * 5;
        }

        public IEntry GetUpgradeA(IJesterRequest request, out int cost)
        {
            var entry = new AttackEntry(Damage + 1, Piercing, ExistingShotCount);
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
            var entry = new AttackEntry(Damage, true, ExistingShotCount);
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