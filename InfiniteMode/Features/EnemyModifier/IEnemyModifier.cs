using System;
using System.Collections.Generic;

namespace InfiniteMode.Features.EnemyModifier;

public interface IEnemyModifier
{
    public double GetWeight(State s, Combat c);
    public int GetCost(State s, Combat c);
    public void Apply(State s, Combat c, int amount, out IEnumerable<Type>? modifiers);
}