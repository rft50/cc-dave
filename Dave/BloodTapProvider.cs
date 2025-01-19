using Dave.Actions;
using Dave.External;

namespace Dave;

public class BloodTapProvider : IBloodTapOptionProvider
{
    public IEnumerable<Status> GetBloodTapApplicableStatuses(State state, Combat combat, IReadOnlySet<Status> allStatuses)
    {
        if (allStatuses.Contains(ModEntry.Instance.RedRigging.Status))
            yield return ModEntry.Instance.RedRigging.Status;
        if (allStatuses.Contains(ModEntry.Instance.BlackRigging.Status))
            yield return ModEntry.Instance.BlackRigging.Status;
        if (allStatuses.Contains(ModEntry.Instance.RedBias.Status))
            yield return ModEntry.Instance.RedBias.Status;
        if (allStatuses.Contains(ModEntry.Instance.BlackBias.Status))
            yield return ModEntry.Instance.BlackBias.Status;
    }

    public IEnumerable<List<CardAction>> GetBloodTapOptionsActions(State state, Combat combat, IReadOnlySet<Status> allStatuses)
    {
        var red = ModEntry.Instance.RedRigging.Status;
        var black = ModEntry.Instance.BlackRigging.Status;
        var hasRed = allStatuses.Contains(red);
        var hasBlack = allStatuses.Contains(black);

        if (hasRed)
            yield return new List<CardAction>
            {
                new AHurt
                {
                    hurtAmount = 1,
                    targetPlayer = true
                },
                new AStatus
                {
                    status = red,
                    statusAmount = 5,
                    targetPlayer = true
                }
            };
        if (hasBlack)
            yield return new List<CardAction>
            {
                new AHurt
                {
                    hurtAmount = 1,
                    targetPlayer = true
                },
                new AStatus
                {
                    status = black,
                    statusAmount = 5,
                    targetPlayer = true
                }
            };
        if (hasRed && hasBlack)
            yield return new List<CardAction>
            {
                new AHurt
                {
                    hurtAmount = 1,
                    targetPlayer = true
                },
                new AStatus
                {
                    status = red,
                    statusAmount = 3,
                    targetPlayer = true
                },
                new AStatus
                {
                    status = black,
                    statusAmount = 3,
                    targetPlayer = true
                }
            };
        if (allStatuses.Contains(ModEntry.Instance.RedBias.Status))
            yield return new List<CardAction>
            {
                new AHurt
                {
                    hurtAmount = 1,
                    targetPlayer = true
                },
                new BiasStatusAction
                {
                    Pow = 1
                }
            };
        if (allStatuses.Contains(ModEntry.Instance.BlackBias.Status))
            yield return new List<CardAction>
            {
                new AHurt
                {
                    hurtAmount = 1,
                    targetPlayer = true
                },
                new BiasStatusAction
                {
                    Pow = -1
                }
            };
    }
}