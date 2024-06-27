using System;
using System.Collections.Generic;
using System.Linq;
using InfiniteMode.Features.DecisionModifier;

namespace InfiniteMode.Features.EnemyModifier;

public class ShieldEnemyModifier : IEnemyModifier
{
    public double GetWeight(State s, Combat c) => 1;

    public int GetCost(State s, Combat c) => 1;

    public void Apply(State s, Combat c, int amount, out IEnumerable<Type>? modifiers)
    {
        modifiers = null;
        if (amount == 0) return;
        c.otherShip.Add(Status.maxShield, amount * 2);
        c.otherShip.Add(Status.shield, amount * 2);
        modifiers = Enumerable.Repeat(typeof(ShieldDecisionModifier), amount);
    }
}