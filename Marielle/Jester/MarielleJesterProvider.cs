using System.Collections.Generic;
using Marielle.ExternalAPI;
using Microsoft.Extensions.Logging;

namespace Marielle.Jester;

public class MarielleJesterProvider : IJesterApi.IProvider
{
    public IEnumerable<(double, IJesterApi.IEntry)> GetEntries(IJesterApi.IJesterRequest request)
    {
        if (!ModEntry.Instance.JesterApi!.HasCharacterFlag("enemyHeat"))
            return [];
        List<(double, IJesterApi.IEntry)> options =
        [
            (1.0, new HeatEnemyEntry { Count = 1 }),
            (1.0, new HeatEnemyEntry { Count = 2 }),
            (1.0, new HeatEnemyEntry { Count = 3 }),
            (1.0, new HeatEnemyEntry { Count = 4 })
        ];
        if (ModEntry.Instance.JesterApi.HasCardFlag("exhaust", request))
            options.AddRange(
            [
                (2.0, new CurseSelfEntry()),
                (4.0, new CurseEnemyEntry()),
                (4.0, new EnflamedEnemyEntry()),
            ]);
        return options;
    }
}

internal class CurseSelfEntry : IJesterApi.IEntry
{
    public IReadOnlySet<string> Tags { get; } = new HashSet<string>
    {
        "defensive",
        "status",
        "mustExhaust",
        "curseSelf"
    };
    public IEnumerable<CardAction> GetActions(State s, Combat c)
    {
        return
        [
            new AStatus
            {
                status = ModEntry.Instance.Curse.Status,
                statusAmount = 1,
                targetPlayer = true
            }
        ];
    }

    public int GetCost() => 6;

    public IEnumerable<(double, IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        return [];
    }

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.Blacklist.Add("curseSelf");
    }
}

internal class CurseEnemyEntry : IJesterApi.IEntry
{
    public IReadOnlySet<string> Tags { get; } = new HashSet<string>
    {
        "offensive",
        "statusEnemy",
        "mustExhaust",
        "curseEnemy"
    };
    public IEnumerable<CardAction> GetActions(State s, Combat c)
    {
        return
        [
            new AStatus
            {
                status = ModEntry.Instance.Curse.Status,
                statusAmount = 1,
                targetPlayer = false
            }
        ];
    }

    public int GetCost() => 8;

    public IEnumerable<(double, IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        return [];
    }

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.Blacklist.Add("curseEnemy");
    }
}

internal class HeatEnemyEntry : IJesterApi.IEntry
{
    private static readonly int[] Costs = [5, 11, 18, 26];
    
    public int Count { get; init; }

    public IReadOnlySet<string> Tags { get; } = new HashSet<string>
    {
        "offensive",
        "statusEnemy",
        "heatEnemy"
    };
    public IEnumerable<CardAction> GetActions(State s, Combat c)
    {
        return
        [
            new AStatus
            {
                status = Status.heat,
                statusAmount = Count,
                targetPlayer = false
            }
        ];
    }

    public int GetCost() => Costs[Count - 1];

    public IEnumerable<(double, IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        if (Count == Costs.Length)
            return [];
        return
        [
            (4.0, new HeatEnemyEntry { Count = Count + 1 })
        ];
    }

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.Blacklist.Add("heatEnemy");
    }
}

internal class EnflamedEnemyEntry : IJesterApi.IEntry
{
    public IReadOnlySet<string> Tags { get; } = new HashSet<string>
    {
        "offensive",
        "statusEnemy",
        "mustExhaust",
        "enflamedEnemy"
    };
    public IEnumerable<CardAction> GetActions(State s, Combat c)
    {
        return
        [
            new AStatus
            {
                status = ModEntry.Instance.Enflamed.Status,
                statusAmount = 1,
                targetPlayer = false
            }
        ];
    }

    public int GetCost() => 20;

    public IEnumerable<(double, IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        return [];
    }

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.Blacklist.Add("enflamedEnemy");
    }
}