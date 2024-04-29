using System.ComponentModel.DataAnnotations;
using Jester.Api;

namespace Jester.Generator.Provider.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class AttackProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        var existingShotCount = request.Entries
            .Count(e => e.Tags.Contains("shot"));

        return Enumerable.Range(1, 5)
            .SelectMany(i => new List<(double, IEntry)>
            {
                (0.2, new AttackEntry
                {
                    Damage = i,
                    Piercing = false,
                    ExistingShotCount = existingShotCount
                }),
                (0.15, new AttackEntry
                {
                    Damage = i,
                    Piercing = true,
                    ExistingShotCount = existingShotCount
                })
            });
    }

    private class AttackEntry : IEntry
    {
        [Required] public int Damage { get; init; }
        [Required] public bool Piercing { get; init; }
        [Required] public int ExistingShotCount { get; init; }

        public IReadOnlySet<string> Tags =>
            new HashSet<string>
            {
                "offensive",
                "attack",
                "shot"
            };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
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
        
        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            var options = new List<(double, IEntry)>
            {
                (1, new AttackEntry
                {
                    Damage = Damage + 1,
                    Piercing = Piercing,
                    ExistingShotCount = ExistingShotCount
                })
            };
            if (!Piercing)
                options.Add((1, new AttackEntry
                    {
                        Damage = Damage,
                        Piercing = true,
                        ExistingShotCount = ExistingShotCount
                    }));
            return options;
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