namespace InfiniteMode.Features.DecisionModifier;

public interface IDecisionModifier
{
    public double GetWeight(State s, Combat c, EnemyDecision decision);
    public int GetCost(State s, Combat c);
    public void Apply(State s, Combat c, int amount, EnemyDecision decision);
}