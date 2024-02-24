using Dave.Actions;
using Dave.External;

namespace Dave;

public class BloodTapProvider : IBloodTapOptionProvider
{
    public IEnumerable<Status> GetBloodTapApplicableStatuses(State state, Combat combat, IReadOnlySet<Status> allStatuses)
    {
        if (allStatuses.Contains((Status) ModManifest.red_rigging!.Id!))
            yield return (Status) ModManifest.red_rigging.Id;
        if (allStatuses.Contains((Status) ModManifest.black_rigging!.Id!))
            yield return (Status) ModManifest.black_rigging.Id;
        if (allStatuses.Contains((Status) ModManifest.red_bias!.Id!))
            yield return (Status) ModManifest.red_bias.Id;
        if (allStatuses.Contains((Status) ModManifest.black_bias!.Id!))
            yield return (Status) ModManifest.black_bias.Id;
    }

    public IEnumerable<List<CardAction>> GetBloodTapOptionsActions(State state, Combat combat, IReadOnlySet<Status> allStatuses)
    {
        var red = (Status)ModManifest.red_rigging!.Id!;
        var black = (Status)ModManifest.black_rigging!.Id!;
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
        if (allStatuses.Contains((Status)ModManifest.red_bias!.Id!))
            yield return new List<CardAction>
            {
                new AHurt
                {
                    hurtAmount = 1,
                    targetPlayer = true
                },
                new BiasStatusAction
                {
                    pow = 1
                }
            };
        if (allStatuses.Contains((Status)ModManifest.black_bias!.Id!))
            yield return new List<CardAction>
            {
                new AHurt
                {
                    hurtAmount = 1,
                    targetPlayer = true
                },
                new BiasStatusAction
                {
                    pow = -1
                }
            };
    }
}