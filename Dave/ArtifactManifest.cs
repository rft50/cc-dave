using CobaltCoreModding.Definitions.ExternalItems;
using CobaltCoreModding.Definitions.ModContactPoints;
using Dave.Artifacts;

namespace Dave;

public partial class ModManifest
{
    private ExternalArtifact chip;
    private ExternalArtifact driveRefund;
    private ExternalArtifact ashtray;
    private ExternalArtifact roulette;
    private ExternalArtifact underdriveGenerator;
    private ExternalArtifact riggedDice;
    
    public void LoadManifest(ICustomEventHub eventHub)
    {
        EventHub = eventHub;

        // state, combat, isRed, isBlack, isRoll
        eventHub.MakeEvent<Tuple<State, Combat, bool, bool, bool>>("Dave.RedBlackRoll");
    }

    public void LoadManifest(IArtifactRegistry registry)
    {
        // THE CHIP
        {
            Chip.neutral = artifact_chip_neutral;
            Chip.both = artifact_chip_both;
            Chip.red = artifact_chip_red;
            Chip.black = artifact_chip_black;
            
            chip = new ExternalArtifact("Dave.Chip", typeof(Chip), artifact_chip_neutral!, ownerDeck: dave_deck!);
            chip.AddLocalisation("CHIP", "This chip indicates the red/black roll result for the last card you played.");
            registry.RegisterArtifact(chip);
        }
        // NORMAL
        {
            DriveRefund.on = artifact_drive_refund_on;
            DriveRefund.off = artifact_drive_refund_off;
            
            driveRefund = new ExternalArtifact("Dave.DriveRefund", typeof(DriveRefund), artifact_underdrive_generator!, ownerDeck: dave_deck!);
            driveRefund.AddLocalisation("DRIVE REFUND", "The first time you lose <c=status>overdrive</c> each turn, draw a card.");
            registry.RegisterArtifact(driveRefund);
        }
        {
            ashtray = new ExternalArtifact("Dave.Ashtray", typeof(Ashtray), artifact_ashtray!, ownerDeck: dave_deck!);
            ashtray.AddLocalisation("ASHTRAY", "Every 5 non-Dave cards you play, add a Perfect Odds to your hand");
            registry.RegisterArtifact(ashtray);
        }
        {
            roulette = new ExternalArtifact("Dave.Roulette", typeof(Roulette), artifact_roulette!, ownerDeck: dave_deck!);
            roulette.AddLocalisation("ROULETTE", "Every 3 red/black cards you play with no Rigging, add a Perfect Odds to your hand");
            registry.RegisterArtifact(roulette);
        }
        // BOSS
        {
            underdriveGenerator = new ExternalArtifact("Dave.UnderdriveGenerator", typeof(UnderdriveGenerator), artifact_underdrive_generator!, ownerDeck: dave_deck!);
            underdriveGenerator.AddLocalisation("UNDERDRIVE GENERATOR", "Gain 1 extra <c=energy>ENERGY</c> every turn. <c=downside>Gain -1 <c=status>overdrive</c> every 2 turns.</c>");
            registry.RegisterArtifact(underdriveGenerator);
        }
        {
            riggedDice = new ExternalArtifact("Dave.RiggedDice", typeof(RiggedDice), artifact_rigged_dice!, ownerDeck: dave_deck!);
            riggedDice.AddLocalisation("RIGGED DICE", "Every other turn, add a Perfect Odds to your hand.");
            registry.RegisterArtifact(riggedDice);
        }
    }
}