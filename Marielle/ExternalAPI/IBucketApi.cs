using System.Collections.Generic;
using Nickel;

#nullable enable
namespace Marielle.ExternalAPI;

public interface IBucketApi
{
	Deck BucketDeck { get; }
	Status IngenuityStatus { get; }
	Status SalvageStatus { get; }
	Status SteamCoverStatus { get; }
}
