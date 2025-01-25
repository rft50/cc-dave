namespace InfiniteMode.Features.DecisionModifier;

public class ShieldDecisionModifier : IDecisionModifier
{
    public double GetWeight(State s, Combat c, EnemyDecision decision) => 0;

    public int GetCost(State s, Combat c) => 0;

    public void Apply(State s, Combat c, int amount, EnemyDecision decision)
    {
        var part = DecisionModifierUtil.PickIntentPart(s.rngAi, c, decision, amount, out var count);
        if (part == null) return;
        decision.intents?.Add(new IntentStatus
        {
            amount = count,
            key = part,
            status = Status.shield,
            targetSelf = true
        });
    }
}