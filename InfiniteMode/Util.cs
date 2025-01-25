using System.Collections.Generic;
using System.Linq;

namespace InfiniteMode;

public static class Util
{
    public static void ApplyToShipUpgrades(State state, IEnumerable<CardAction> actions)
    {
        if (state.route is ShipUpgrades shipUpgrades)
        {
            shipUpgrades.actionQueue.AddRange(actions);
            return;
        }
        if (state.rewardsQueue.FirstOrDefault(a => a is AShipUpgrades) is not AShipUpgrades upgrades)
        {
            upgrades = new AShipUpgrades();
            state.rewardsQueue.Insert(0, upgrades);
        }
        
        upgrades.actions.AddRange(actions);
    }
}