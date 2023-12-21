using Jester.Cards;

namespace Jester.Artifacts;

[ArtifactMeta(unremovable = true)]
public class ClosingCeremony : Artifact
{
    public int Count;

    public override void OnCombatStart(State state, Combat combat)
    {
        if (Count == 0) return;
        if (state.map.GetCurrent().contents is not MapBattle { battleType: BattleType.Boss }) return;

        combat.QueueImmediate(new AAddCard
        {
            card = new FinalAct
            {
                upgrade = Upgrade.B
            },
            destination = CardDestination.Deck,
            amount = Count
        });

        Count = 0;
    }

    public override int? GetDisplayNumber(State s)
    {
        return Count == 0 ? null : Count;
    }

    public override List<Tooltip>? GetExtraTooltips() => new()
    {
        new TTCard
        {
            card = new FinalAct
            {
                upgrade = Upgrade.B
            }
        }
    };
}