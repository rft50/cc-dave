using Jester.Api;

namespace Jester.Generator.Provider.Drake;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;
using IProvider = IJesterApi.IProvider;

public class SerenityProvider : IProvider
{
    public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request)
    {
        if (!ModManifest.JesterApi.HasCharacterFlag("heat")) return new List<(double, IEntry)>();

        return new List<(double, IEntry)>
        {
            (2, new SerenityEntry())
        };
    }
    
    private class SerenityEntry : IEntry
    {
        public IReadOnlySet<string> Tags { get; } = new HashSet<string>
        {
            "status",
            "heat",
            "serenity"
        };

        public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AStatus
            {
                status = Enum.Parse<Status>("serenity"),
                statusAmount = 1,
                targetPlayer = true
            }
        };

        public int GetCost() => 15;

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            return new List<(double, IEntry)>();
        }

        public void AfterSelection(IJesterRequest request)
        {
            request.Blacklist.Add("heat");
        }
    }
}