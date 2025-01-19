using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Marielle.ExternalAPI;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;

namespace Marielle.Features;

[HarmonyPatch]
public class StatusManager : IStatusLogicHook
{
    public StatusManager()
    {
        ModEntry.Instance.KokoroApi.RegisterStatusLogicHook(this, 0);
    }

    public void OnStatusTurnTrigger(State state, Combat combat, StatusTurnTriggerTiming timing, Ship ship, Status status,
        int oldAmount, int newAmount)
    {
        if (timing == StatusTurnTriggerTiming.TurnStart && status == ModEntry.Instance.Enflamed.Status && ship.Get(ModEntry.Instance.Enflamed.Status) > 0)
        {
            combat.Queue(new AStatus
            {
                status = Status.heat,
                statusAmount = ship.Get(ModEntry.Instance.Enflamed.Status),
                targetPlayer = ship == state.ship
            });
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AStatus), "Begin")]
    private static void AStatus_Begin_Prefix(AStatus __instance, State s, out int __state)
    {
        var ship = GetShip(__instance, s);
        __state = ship.Get(ModEntry.Instance.Curse.Status);
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(AStatus), "Begin")]
    private static void AStatus_Begin_Postfix(AStatus __instance, State s, int __state)
    {
        var ship = GetShip(__instance, s);
        var diff = ship.Get(ModEntry.Instance.Curse.Status) - __state;
        ship.heatTrigger += diff;
        ship.overheatDamage += diff;
    }

    private static Ship GetShip(AStatus instance, State s)
    {
        return instance.targetPlayer ? s.ship : ((Combat)s.route).otherShip;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Ship), "ResetAfterCombat")]
    private static void Ship_ResetAfterCombat_Prefix(Ship __instance)
    {
        __instance.heatTrigger -= __instance.Get(ModEntry.Instance.Curse.Status);
        __instance.overheatDamage -= __instance.Get(ModEntry.Instance.Curse.Status);
    }
}