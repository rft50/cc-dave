using Jester.Api;

namespace Jester.Generator;

public class JesterRequest : IJesterRequest
{
    // provided by caller
    public int Seed { get; set; }
    public string? FirstAction { get; set; }
    public State State { get; set; } = null!;
    public int BasePoints { get; set; }
    public CardData CardData { get; set; }
    public int ActionLimit { get; set; }
    public bool SingleUse { get; set; }

    // calculation
    public Random Random { get; set; } = null!;
    public IList<IEntry> Entries { get; set; } = new List<IEntry>();
    public ISet<string> Blacklist { get; set; } = new HashSet<string>();
    public ISet<string> Whitelist { get; set; } = new HashSet<string>();
    public ISet<int> OccupiedMidrow { get; set; } = new HashSet<int>();
    public int MinCost { get; set; }
    public int MaxCost { get; set; }
    public IDictionary<string, object> Data { get; set; } = new Dictionary<string, object>(); // misc data for your magical needs
}