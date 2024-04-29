using System.ComponentModel.DataAnnotations;
using Jester.Api;

namespace Jester.Generator.Provider.Common;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class DrawProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        if (request.CardData.cost == 0 && !ModManifest.JesterApi.HasCardFlag("exhaust", request)) return new List<(double, IEntry)>();
        
        return Enumerable.Range(1, 3)
            .Select(i => (0.3, new DrawEntry
            {
                Count = i
            } as IEntry));
    }
    
    private class DrawEntry : IEntry
    {
        [Required] public int Count { get; init; }
        public IReadOnlySet<string> Tags { get; } = new HashSet<string>
        {
            "utility",
            "draw"
        };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new ADrawCard
            {
                count = Count
            }
        };

        public int GetCost() => 8 * Count;

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            if (Count >= 7) return new List<(double, IEntry)>();
            return new List<(double, IEntry)>
            {
                (1, new DrawEntry
                {
                    Count = Count + 1
                })
            };
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("draw");
        }
    }
}