using Jester.Api;

namespace Jester.Generator;

public class JesterResult : IJesterResult
{
    public IList<IEntry> Entries { get; set; } = null!;
    public CardData CardData { get; set; }
}