using CobaltCoreModding.Definitions.ExternalItems;

namespace Dave.Artifacts;

// Chip: This chip indicates the red/black roll result for the last card you played.

[ArtifactMeta(unremovable = true)]
public class Chip : Artifact
{
    public static ExternalSprite red;
    public static ExternalSprite black;
    public static ExternalSprite both;
    public static ExternalSprite neutral;

    private ExternalSprite sprite = neutral;

    public Chip()
    {
        if (ModManifest.EventHub != null)
            ModManifest.EventHub.ConnectToEvent<Tuple<State, Combat, bool, bool, bool>>("Dave.RedBlackRoll", OnRoll);
    }

    public override void OnRemoveArtifact(State state)
    {
        ModManifest.EventHub.DisconnectFromEvent<Tuple<State, Combat, bool, bool, bool>>("Dave.RedBlackRoll", OnRoll);
    }

    public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition,
        int handCount)
    {
        sprite = neutral;
    }

    public override void OnCombatEnd(State state)
    {
        sprite = neutral;
    }

    private void OnRoll(Tuple<State, Combat, bool, bool, bool> data)
    {
        var (state, _, isRed, isBlack, _) = data;

        if (!ArtifactUtil.PlayerHasArtifact(state, this))
        {
            ModManifest.EventHub.DisconnectFromEvent<Tuple<State, Combat, bool, bool, bool>>("Dave.RedBlackRoll", OnRoll);
            return;
        }

        sprite = isRed switch
        {
            true when isBlack => both,
            true => red,
            _ => black
        };
        
        Pulse();
    }

    public override Spr GetSprite()
    {
        return (Spr) sprite.Id!;
    }
}