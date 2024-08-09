using System.Collections.Generic;

namespace Marielle.ExternalAPI;

public partial interface IJesterApi
{
    public void RegisterProvider(IProvider provider);

    public interface IProvider
    {
        // weight, entry
        public IEnumerable<(double, IEntry)> GetEntries(IJesterRequest request);
    }

    public interface IEntry
    {
        public IReadOnlySet<string> Tags { get; }

        public IEnumerable<CardAction> GetActions(State s, Combat c);

        public int GetCost();

        // weight, entry
        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir);

        public void AfterSelection(IJesterRequest request);
    }

    public interface IJesterRequest
    {
        // provided by caller
        public int Seed { get; set; }
        public string? FirstAction { get; set; }
        public State State { get; set; }
        public int BasePoints { get; set; }
        public CardData CardData { get; set; }
        public int ActionLimit { get; set; }
        public bool SingleUse { get; set; }
        public CardMeta CardMeta { get; set; }

        // calculation
        public Rand Random { get; set; }
        public IList<IEntry> Entries { get; set; }
        public ISet<string> Blacklist { get; set; }
        public ISet<string> Whitelist { get; set; }
        public ISet<int> OccupiedMidrow { get; set; }
        public int MinCost { get; set; }
        public int MaxCost { get; set; }
        public IDictionary<string, object> Data { get; set; } // misc data for your magical needs
    }
}