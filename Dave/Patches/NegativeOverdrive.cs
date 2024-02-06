using Dave.External;
using HarmonyLib;

namespace Dave.Patches;

[HarmonyPatch]
internal sealed class NegativeOverdriveManager : IStatusLogicHook
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(Ship), "CanBeNegative")]
	private static void Ship_CanBeNegative_Postfix(Status status, ref bool __result)
	{
		if (status == Status.overdrive)
			__result = true;
	}
}
