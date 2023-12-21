using HarmonyLib;

namespace Jester.Patches;

[HarmonyPatch(typeof(Ship))]
public class ShipPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("OnBeginTurn")]
    public static void OnBeginTurn(Ship __instance, Combat c)
    {
        if (__instance.Get(Enum.Parse<Status>("timeStop")) == 0)
        {
            if (__instance.Get((Status)ModManifest.OpeningFatigue.Id!) > 0)
            {
                c.QueueImmediate(new AStatus
                {
                    targetPlayer = true,
                    statusAmount = -1,
                    status = (Status)ModManifest.OpeningFatigue.Id!
                });
            }
        }

        if (__instance.isPlayerShip && __instance.Get((Status)ModManifest.OpeningFatigue.Id!) > 0)
            c.energy--;
    }
}