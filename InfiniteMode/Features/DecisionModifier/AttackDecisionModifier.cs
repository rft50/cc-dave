using System.Collections.Generic;
using System.Linq;

namespace InfiniteMode.Features.DecisionModifier;

public class AttackDecisionModifier : IDecisionModifier
{
    public double GetWeight(State s, Combat c, EnemyDecision decision) => decision.intents != null ? decision.intents.Any(i => i is IntentAttack) ? 1.0 : 0.0 : 0.0;

    public int GetCost(State s, Combat c) => 1;

    public void Apply(State s, Combat c, int amount, EnemyDecision decision)
    {
        if (decision.intents == null) return;
        
        var toSpend = amount * 2;
        var options = new Dictionary<IntentAttack, int>();

        foreach (var intent in decision.intents.OfType<IntentAttack>())
        {
            var count = c.otherShip.parts.Count(p => p.key == intent.key) * intent.multiHit;
            if (count > 0)
                options[intent] = count;
        }
        
        if (options.Count == 0) return;

        while (toSpend > 0)
        {
            var opts = options
                .Where(o => o.Value <= toSpend)
                .ToList();
            if (opts.Count == 0) break;
            var option = opts.Random(s.rngAi);

            option.Key.damage++;
            toSpend -= option.Value;
        }

        if (toSpend == amount * 2)
        {
            options.Keys.First().damage++;
        }
    }
}