using HarmonyLib;

namespace Dave
{
    [HarmonyPatch(typeof(Ship))]
    public class ShipPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("CanBeNegative")]
        public static void CanBeNegative(Status status, ref bool __result)
        {
            if (status == Status.overdrive)
                __result = true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("OnAfterTurn")]
        public static void OnAfterTurn(Ship __instance, Combat c)
        {
            if (__instance.Get(Status.timeStop) <= 0 && __instance.Get(Status.overdrive) < 0)
                c.QueueImmediate(new AStatus
                {
                    status = Status.overdrive,
                    statusAmount = 1,
                    targetPlayer = __instance.isPlayerShip
                });
        }
    }
}