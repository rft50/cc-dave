using Jester.Actions;

namespace Jester.Artifacts;

[ArtifactMeta(unremovable = true)]
public class OpeningScript : Artifact
{
    public List<Tuple<int, bool>> CardData = new();
    private int _currentCost;

    public void RegisterCard(Card card, bool discounted)
    {
        CardData.Add(new Tuple<int, bool>(card.uuid, discounted));
        UpdateCache(StateExt.Instance!);
    }

    private void UpdateCache(State state)
    {
        var cards = CardData.Select(d => state.FindCard(d.Item1)).ToList();

        for (var i = cards.Count - 1; i >= 0; i--)
        {
            if (cards[i] != null) continue;
            cards.RemoveAt(i);
            CardData.RemoveAt(i);
        }

        _currentCost = cards.Select(c => c!.GetData(state).cost).Sum();
        _currentCost -= CardData.Count(d => d.Item2);
        if (_currentCost < 0)
            _currentCost = 0;
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        UpdateCache(state);
        if (_currentCost > 0)
        {
            combat.Queue(new AStatus
            {
                targetPlayer = true,
                statusAmount = _currentCost,
                status = (Status)ModManifest.OpeningFatigue.Id!
            });
        }
        combat.Queue(CardData
            .Select(d => state.FindCard(d.Item1))
            .Where(c => c != null)
            .Select(c =>
            {
                var action = ModManifest.KokoroApi.Actions.MakePlaySpecificCardFromAnywhere(c!.uuid);

                action.artifactPulse = Key();
                
                return action;
            }));
        
    }

    public override int? GetDisplayNumber(State s)
    {
        if (_currentCost == 0)
            UpdateCache(s);
        return _currentCost == 0 ? null : _currentCost;
    }

    public override List<Tooltip> GetExtraTooltips() => new()
    {
        ModManifest.OpeningScriptedTooltip
    };
}