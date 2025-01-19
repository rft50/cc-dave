using Dave.Actions;
using Dave.External;

namespace Dave.Jester;

public class DaveJesterProvider : IJesterApi.IProvider
{
    public IEnumerable<(double, IJesterApi.IEntry)> GetEntries(IJesterApi.IJesterRequest request)
    {
        if (request.Whitelist.Contains("cost"))
            return
            [
                (0.33, new JesterShieldHurtCostEntry { Count = 1 }),
                (0.33, new JesterShieldHurtCostEntry { Count = 2 }),
                (0.33, new JesterShieldHurtCostEntry { Count = 3 })
            ];
        
        if (ModEntry.Instance.JesterApi!.HasCharacterFlag("dave_rigging"))
            return [
                (4, new JesterRedRiggingEntry { Count = 1 }),
                (4, new JesterBlackRiggingEntry { Count = 1 })
            ];

        return [];
    }
}

internal class JesterRedRiggingEntry : IJesterApi.IEntry
{
    public int Count { get; init; }
    
    public IReadOnlySet<string> Tags => new HashSet<string>
    {
        "status",
        "red_rigging"
    };

    public IEnumerable<CardAction> GetActions(State s, Combat c) =>
    [
        new AStatus
        {
            status = ModEntry.Instance.RedRigging.Status,
            statusAmount = Count,
            targetPlayer = true
        }
    ];

    public int GetCost() => 8 * Count;

    public IEnumerable<(double, IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        if (Count >= 3) return [];
        return
        [
            (1, new JesterRedRiggingEntry
            {
                Count = Count + 1
            })
        ];
    }

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.Blacklist.Add("red_rigging");
    }
}

internal class JesterBlackRiggingEntry : IJesterApi.IEntry
{
    public int Count { get; init; }
    
    public IReadOnlySet<string> Tags => new HashSet<string>
    {
        "status",
        "black_rigging"
    };

    public IEnumerable<CardAction> GetActions(State s, Combat c) =>
    [
        new AStatus
        {
            status = ModEntry.Instance.RedRigging.Status,
            statusAmount = Count,
            targetPlayer = true
        }
    ];

    public int GetCost() => 8 * Count;

    public IEnumerable<(double, IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        if (Count >= 3) return [];
        return
        [
            (1, new JesterBlackRiggingEntry
            {
                Count = Count + 1
            })
        ];
    }

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.Blacklist.Add("black_rigging");
    }
}



internal class JesterShieldHurtCostEntry : IJesterApi.IEntry
{
    public int Count { get; init; }
    
    public IReadOnlySet<string> Tags => new HashSet<string>
    {
        "cost",
        "shield_hurt"
    };

    public IEnumerable<CardAction> GetActions(State s, Combat c) =>
    [
        new ShieldHurtAction
        {
            hurtAmount = Count
        }
    ];

    public int GetCost() => -12 * Count;

    public IEnumerable<(double, IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        if (Count == 1) return [];
        return
        [
            (1, new JesterShieldHurtCostEntry
            {
                Count = Count - 1
            })
        ];
    }

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.Blacklist.Add("shield_hurt");
    }
}