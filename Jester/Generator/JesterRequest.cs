namespace Jester.Generator;

public class JesterRequest
{
    // provided by caller
    public int Seed;
    public string? FirstAction;
    public State State = null!;
    public int BasePoints;
    public CardData CardData;

    // calculation
    public Random Random = null!;
    public List<IEntry> Entries = new();
    public HashSet<string> Blacklist = new();
    public HashSet<string> Whitelist = new();
    public HashSet<int> OccupiedMidrow = new();
    public int MinCost;
    public int MaxCost;
    public HashSet<object> Data = new(); // misc data for your magical needs
}