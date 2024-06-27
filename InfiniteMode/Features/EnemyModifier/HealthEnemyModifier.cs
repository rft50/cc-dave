using System;
using System.Collections.Generic;

namespace InfiniteMode.Features.EnemyModifier;

public class HealthEnemyModifier : IEnemyModifier
{
    public double GetWeight(State s, Combat c) => 1;

    public int GetCost(State s, Combat c) => 1;

    public void Apply(State s, Combat c, int amount, out IEnumerable<Type>? modifiers)
    {
        modifiers = null;
        if (amount == 0) return;
        var diff = c.otherShip.hullMax * amount / 10;
        c.otherShip.hullMax += diff;
        c.otherShip.hull += diff;
    }
}