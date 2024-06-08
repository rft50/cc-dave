using Marielle.ExternalAPI;

namespace Marielle.Features;

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
}