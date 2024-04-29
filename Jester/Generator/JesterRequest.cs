using System.ComponentModel.DataAnnotations;
using Jester.Api;

namespace Jester.Generator;

using IJesterRequest = IJesterApi.IJesterRequest;
using IEntry = IJesterApi.IEntry;

public class JesterRequest : IJesterRequest
{
    // provided by caller
    [Required] public int Seed { get; set; }
    [Required] public string? FirstAction { get; set; }
    [Required] public State State { get; set; } = null!;
    [Required] public int BasePoints { get; set; }
    [Required] public CardData CardData { get; set; }
    [Required] public int ActionLimit { get; set; }
    [Required] public bool SingleUse { get; set; }
    [Required] public CardMeta CardMeta { get; set; } = null!;

    // calculation
    public Rand Random { get; set; } = null!;
    public IList<IEntry> Entries { get; set; } = new List<IEntry>();
    public ISet<string> Blacklist { get; set; } = new HashSet<string>();
    public ISet<string> Whitelist { get; set; } = new HashSet<string>();
    public ISet<int> OccupiedMidrow { get; set; } = new HashSet<int>();
    public int MinCost { get; set; }
    public int MaxCost { get; set; }
    public IDictionary<string, object> Data { get; set; } = new Dictionary<string, object>(); // misc data for your magical needs
}