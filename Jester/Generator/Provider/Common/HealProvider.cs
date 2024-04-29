using System.ComponentModel.DataAnnotations;
using Jester.Api;

namespace Jester.Generator.Provider.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class HealProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        var cost = 35;
        if (!ModManifest.JesterApi.HasCardFlag("exhaust", request)) cost *= 2;

        if (ModManifest.JesterApi.HasCardFlag("singleUse", request))
            return new List<(double, IEntry)>();

        return new List<(double, IEntry)>
        {
            (0.5, new HealEntry
            {
                CostPer = cost,
                Count = 1
            })
        };
    }
    
    private class HealEntry : IEntry
    {
        [Required] public int CostPer { get; init; }
        [Required] public int Count { get; init; }
        
        public IReadOnlySet<string> Tags =>
            new HashSet<string>
            {
                "defensive",
                "heal"
            };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AHeal
            {
                healAmount = 1,
                targetPlayer = true,
                canRunAfterKill = true
            }
        };

        public int GetCost() => CostPer;

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            if (Count > 1)
                return new List<(double, IEntry)>();
            return new List<(double, IEntry)>
            {
                (0.1, new HealEntry
                {
                    CostPer = CostPer,
                    Count = Count + 1
                })
            };
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("heal");
        }
    }
}