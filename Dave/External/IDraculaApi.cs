using Nickel;

namespace Dave.External;

public interface IDraculaApi
{
	IDeckEntry DraculaDeck { get; }
	IStatusEntry BleedingStatus { get; }
	void RegisterBloodTapOptionProvider(IBloodTapOptionProvider provider, double priority = 0);
}

public interface IBloodTapOptionProvider
{
	IEnumerable<Status> GetBloodTapApplicableStatuses(State state, Combat combat, IReadOnlySet<Status> allStatuses);
	IEnumerable<List<CardAction>> GetBloodTapOptionsActions(State state, Combat combat, IReadOnlySet<Status> allStatuses);
}