using Dave.External;
using HarmonyLib;

namespace Dave.Patches;

[HarmonyPatch]
internal sealed class NegativeOverdriveManager : IStatusLogicHook
{
	public NegativeOverdriveManager()
	{
		ModManifest.KokoroApi.RegisterStatusLogicHook(this, 0);
	}

	public bool HandleStatusTurnAutoStep(State state, Combat combat, StatusTurnTriggerTiming timing, Ship ship, Status status, ref int amount, ref StatusTurnAutoStepSetStrategy setStrategy)
	{
		if (status != Status.overdrive)
			return false;
		if (timing != StatusTurnTriggerTiming.TurnEnd)
			return false;

		if (amount < 0)
			amount++;
		return true;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(Ship), "CanBeNegative")]
	private static void Ship_CanBeNegative_Postfix(Status status, ref bool __result)
	{
		if (status == Status.overdrive)
			__result = true;
	}
}
