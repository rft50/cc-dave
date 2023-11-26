using Dave.Actions;

namespace Dave.Cards;

// 1-cost, Red to 3 damage shot, Black to 1 damage shot
// A: Black shot now always fires
// B: Red is now two 2 damage shots
[CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class LuckyShotCard : Card
{
    private static Spr card_sprite = Spr.cards_GoatDrone;

    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions;
        List<CardAction> builtActions;

        switch (upgrade)
        {
            default:
                builtActions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                {
                    new AAttack { damage = this.GetDmg(s, 3) }
                }, new List<CardAction>
                {
                    new AAttack { damage = this.GetDmg(s, 1) }
                });

                actions = new List<CardAction>
                {
                    builtActions[0],
                    builtActions[1],
                    builtActions[2],
                    new ADummyAction()
                };
                break;
            case Upgrade.A:
                builtActions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                {
                    new AAttack { damage = this.GetDmg(s, 3) }
                });

                actions = new List<CardAction>
                {
                    builtActions[0],
                    builtActions[1],
                    new AAttack { damage = this.GetDmg(s, 1) },
                    new ADummyAction()
                };
                break;
            case Upgrade.B:
                builtActions = RandomChoiceActionFactory.BuildActions(new List<CardAction>
                {
                    new AAttack { damage = this.GetDmg(s, 2) },
                    new AAttack { damage = this.GetDmg(s, 2) }
                }, new List<CardAction>
                {
                    new AAttack { damage = this.GetDmg(s, 1) }
                });

                actions = new List<CardAction>
                {
                    builtActions[0],
                    builtActions[1],
                    builtActions[2],
                    builtActions[3],
                    new ADummyAction()
                };
                break;
        }
        
        return actions;
    }

    public override CardData GetData(State state) => new()
    {
        cost = 1,
        art = card_sprite
    };
}