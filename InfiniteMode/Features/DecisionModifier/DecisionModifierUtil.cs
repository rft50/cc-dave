using System.Collections.Generic;
using System.Linq;

namespace InfiniteMode.Features.DecisionModifier;

public class DecisionModifierUtil
{
    public static IEnumerable<Part> GetOpenNames(Combat c, EnemyDecision decision)
    {
        return c.otherShip.parts
            .Where(p => p.key != null && (!decision.intents?.Any(i => i.key == p.key) ?? true))
            .Where(p => p.type != PType.empty);
    }

    public static IEnumerable<Part> GetOpenNames(Combat c, EnemyDecision decision, PType type, bool forced)
    {
        var options = c.otherShip.parts
            .Where(p => p.key != null && (!decision.intents?.Any(i => i.key == p.key) ?? true))
            .ToList();
        if (forced || options.Any(p => p.type == type))
        {
            return options
                .Where(p => p.type == type);
        }
        return options
            .Where(p => p.type != PType.empty || type == PType.empty);
    }

    public static string? PickIntentPart(Rand rng, IEnumerable<Part> parts, int budget, out int count)
    {
        var counts = parts
            .Select(p => p.key)
            .Where(p => p != null)
            .GroupBy(g => g!)
            .Where(g => budget % g.Count() == 0)
            .ToDictionary(g => g.Key, g => g.Count());
        if (counts.Count == 0)
        {
            count = 0;
            return null;
        }

        var option = counts.Keys.ToList().Random(rng);
        count = budget / counts[option];
        return option;
    }
    
    public static string? PickIntentPart(Rand rng, Combat c, EnemyDecision decision, int budget, out int count)
    {
        return PickIntentPart(rng, GetOpenNames(c, decision), budget, out count);
    }
    
    public static string? PickIntentPart(Rand rng, Combat c, EnemyDecision decision, PType type, int budget, out int count)
    {
        return PickIntentPart(rng, GetOpenNames(c, decision, type, true), budget, out count)
            ?? PickIntentPart(rng, GetOpenNames(c, decision, type, false), budget, out count);
    }
}