using Dave.Actions;

namespace Dave.Cards;

// 1-cost, shoot once, consume Red to shoot once, rig Black
// A: red shot is 2 damage
// B: red has a second, 0-damage shot
[CardMeta(rarity = Rarity.common, upgradesTo = new [] { Upgrade.A, Upgrade.B })]
public class RiggingShotCard : Card
{
    public override List<CardAction> GetActions(State s, Combat c)
    {
        var actions = new List<CardAction>();
        List<CardAction> builtActions;

        switch (upgrade)
        {
            default:
                builtActions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                {
                    new AAttack { damage = this.GetDmg(s, 1), fast = true }
                }, new List<CardAction>
                {
                    new AStatus
                    {
                        status = ModEntry.Instance.BlackRigging.Status,
                        targetPlayer = true,
                        statusAmount = 1,
                        mode = AStatusMode.Add
                    }
                });
                actions.Add(builtActions[0]);
                actions.Add(new AAttack { damage = this.GetDmg(s, 1), fast = true });
                actions.Add(builtActions[1]);
                actions.Add(builtActions[2]);
                actions.Add(new ADummyAction());
                break;
            case Upgrade.A:
                builtActions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                {
                    new AAttack { damage = this.GetDmg(s, 2), fast = true }
                }, new List<CardAction>
                {
                    new AStatus
                    {
                        status = ModEntry.Instance.BlackRigging.Status,
                        targetPlayer = true,
                        statusAmount = 1,
                        mode = AStatusMode.Add
                    }
                });
                actions.Add(builtActions[0]);
                actions.Add(new AAttack { damage = this.GetDmg(s, 1), fast = true });
                actions.Add(builtActions[1]);
                actions.Add(builtActions[2]);
                actions.Add(new ADummyAction());
                break;
            case Upgrade.B:
                builtActions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                {
                    new AAttack { damage = this.GetDmg(s, 1), fast = true },
                    new AAttack { damage = this.GetDmg(s, 0), fast = true }
                }, new List<CardAction>
                {
                    new AStatus
                    {
                        status = ModEntry.Instance.BlackRigging.Status,
                        targetPlayer = true,
                        statusAmount = 2,
                        mode = AStatusMode.Add
                    }
                });
                actions.Add(builtActions[0]);
                actions.Add(new AAttack { damage = this.GetDmg(s, 1), fast = true });
                actions.Add(builtActions[1]);
                actions.Add(builtActions[2]);
                actions.Add(builtActions[3]);
                actions.Add(new ADummyAction());
                break;
        }

        return actions;
    }

    public override CardData GetData(State state) => new()
    {
        cost = 1
    };
}