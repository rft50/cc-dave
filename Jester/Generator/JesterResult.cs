using Jester.Api;

namespace Jester.Generator;

using IJesterResult = IJesterApi.IJesterResult;
using IEntry = IJesterApi.IEntry;

public class JesterResult : IJesterResult
{
    public IList<IEntry> Entries { get; set; } = null!;
    public CardData CardData { get; set; }
    public int SparePoints { get; set; }
}