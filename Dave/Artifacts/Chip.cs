
using Dave.Api;

namespace Dave.Artifacts;

// Chip: This chip indicates the red/black roll result for the last card you played.

[ArtifactMeta(unremovable = true)]
public class Chip : Artifact, IDaveApi.IRollHook
{
    internal static Spr Red;
    internal static Spr Black;
    internal static Spr Both;
    internal static Spr Neutral;

    private Spr _sprite = Neutral;

    public Chip()
    {
        ModEntry.Instance.RollHookManager.Register(this, 0);
    }

    public override void OnRemoveArtifact(State state)
    {
        ModEntry.Instance.RollHookManager.Unregister(this);
    }

    public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition,
        int handCount)
    {
        _sprite = Neutral;
    }

    public override void OnCombatEnd(State state)
    {
        _sprite = Neutral;
    }

    public void OnRoll(State state, Combat combat, bool isRed, bool isBlack, bool isRoll)
    {
        if (!ArtifactUtil.PlayerHasArtifact(state, this))
        {
            ModEntry.Instance.RollHookManager.Unregister(this);
            return;
        }

        _sprite = isRed switch
        {
            true when isBlack => Both,
            true => Red,
            _ => Black
        };
        
        Pulse();
    }

    public override Spr GetSprite()
    {
        return _sprite;
    }
}