namespace InfiniteMode.ExternalAPI;

public partial interface IKokoroApi
{
	IActionApi Actions { get; }

	public interface IActionApi
	{
		AVariableHint SetTargetPlayer(AVariableHint action, bool targetPlayer);
	}
}