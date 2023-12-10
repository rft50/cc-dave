using CobaltCoreModding.Definitions;
using CobaltCoreModding.Definitions.ExternalItems;
using CobaltCoreModding.Definitions.ModContactPoints;
using CobaltCoreModding.Definitions.ModManifests;
using CobaltCoreModding.Definitions.OverwriteItems;
using Jester.Actions;
using Jester.Cards;
using Microsoft.Extensions.Logging;

namespace Jester
{
    public class ModManifest : IModManifest, ISpriteManifest, IAnimationManifest, IDeckManifest, ICardManifest, ICardOverwriteManifest, ICharacterManifest, IGlossaryManifest, IArtifactManifest, IStatusManifest, ICustomEventManifest
    {
        public static ExternalStatus? demo_status;
        internal static ICustomEventHub? EventHub;
        private ExternalSprite? card_art_sprite;
        private ExternalAnimation? default_animation;
        private ExternalSprite? demo_status_sprite;
        private ExternalSprite? DemoAttackSprite;
        private ExternalDeck? dracula_deck;
        private ExternalSprite? dracular_art;
        private ExternalSprite? dracular_border;
        private ExternalAnimation? mini_animation;
        private ExternalSprite? mini_dracula_sprite;
        private ExternalSprite? pinker_per_border_over_sprite;

        public IEnumerable<DependencyEntry> Dependencies => new DependencyEntry[0];
        public DirectoryInfo? GameRootFolder { get; set; }
        public ILogger? Logger { get; set; }
        public DirectoryInfo? ModRootFolder { get; set; }
        public string Name => "EWanderer.DemoMod.MainManifest";

        public void BootMod(IModLoaderContact contact)
        {
            //Nothing to do here lol.
        }

        public void LoadManifest(ISpriteRegistry artRegistry)
        {
            if (ModRootFolder == null)
                throw new Exception("No root folder set!");
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("patched_cobalt_core.png"));
                var sprite = new ExternalSprite("EWanderer.DemoMod.Patched_Cobalt_Core", new FileInfo(path));
                artRegistry.RegisterArt(sprite, (int)Spr.cockpit_cobalt_core);
            }

            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("border_over_pinker_peri.png"));
                pinker_per_border_over_sprite = new ExternalSprite("EWanderer.DemoMod.PinkerPeri.BorderOver", new FileInfo(path));
                if (!artRegistry.RegisterArt(pinker_per_border_over_sprite))
                    throw new Exception("Cannot register sprite.");
            }

            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("Shield.png"));
                card_art_sprite = new ExternalSprite("EWanderer.DemoMod.DemoCardArt", new FileInfo(path));
                if (!artRegistry.RegisterArt(card_art_sprite))
                    throw new Exception("Cannot register sprite.");
                EWandererDemoCard.card_sprite = (Spr)(card_art_sprite.Id ?? throw new NullReferenceException());
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("dracula_mini_0.png"));
                mini_dracula_sprite = new ExternalSprite("EWanderer.DemoMod.dracular.mini", new FileInfo(path));
                if (!artRegistry.RegisterArt(mini_dracula_sprite))
                    throw new Exception("Cannot register sprite.");
                EWandererDemoCard.card_sprite = (Spr)(mini_dracula_sprite.Id ?? throw new NullReferenceException());
            }

            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("demo_status.png"));
                demo_status_sprite = new ExternalSprite("EWanderer.DemoMod.demo_status.sprite", new FileInfo(path));
                if (!artRegistry.RegisterArt(demo_status_sprite))
                    throw new Exception("Cannot register sprite.");
            }
        }

        public void LoadManifest(IAnimationRegistry registry)
        {
            default_animation = new ExternalAnimation("ewanderer.demomod.dracula.neutral", dracula_deck ?? throw new NullReferenceException(), "neutral", false, new ExternalSprite[] {
                ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_0),
                ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_1),
                ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_2),
                ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_3),
                ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_4),
            });

            registry.RegisterAnimation(default_animation);
            if (mini_dracula_sprite == null)
                throw new Exception();

            mini_animation = new ExternalAnimation("ewanderer.demomod.dracula.mini", dracula_deck, "mini", false, new ExternalSprite[] { mini_dracula_sprite });

            registry.RegisterAnimation(mini_animation);
        }

        public void LoadManifest(IDeckRegistry registry)
        {
            dracular_art = ExternalSprite.GetRaw((int)Spr.cards_colorless);
            dracular_border = ExternalSprite.GetRaw((int)Spr.cardShared_border_dracula);

            dracula_deck = new ExternalDeck("EWanderer.Demomod.DraculaDeck", System.Drawing.Color.Crimson, System.Drawing.Color.Purple, dracular_art ?? throw new NullReferenceException(), dracular_border ?? throw new NullReferenceException(), null);

            if (!registry.RegisterDeck(dracula_deck))
                return;
        }

        public void LoadManifest(ICardRegistry registry)
        {
            if (card_art_sprite == null)
                return;

            var joker = new ExternalCard("rft.Jester.Joker", typeof(Joker), card_art_sprite, null);
            joker.AddLocalisation("Joker");
            registry.RegisterCard(joker);
        }

        public void LoadManifest(ICardOverwriteRegistry registry)
        {
        }

        public void LoadManifest(ICharacterRegistry registry)
        {
            var dracular_spr = ExternalSprite.GetRaw((int)Spr.panels_char_colorless);

            var start_cards = new Type[] { typeof(DraculaCard), typeof(DraculaCard) };
            var playable_dracular_character = new ExternalCharacter("EWanderer.DemoMod.DracularChar", dracula_deck ?? throw new NullReferenceException(), dracular_spr, start_cards, new Type[0], default_animation ?? throw new NullReferenceException(), mini_animation ?? throw new NullReferenceException());
            playable_dracular_character.AddNameLocalisation("Count Dracula");
            playable_dracular_character.AddDescLocalisation("A vampire using blood magic to invoke the powers of the void.");
            registry.RegisterCharacter(playable_dracular_character);
        }

        public void LoadManifest(IGlossaryRegisty registry)
        {
            var icon = ExternalSprite.GetRaw((int)Spr.icons_ace);
            var glossary = new ExternalGlossary("Ewanderer.DemoMod.DemoCard.Glossary", "ewandererdemocard", false, ExternalGlossary.GlossayType.action, icon);
            glossary.AddLocalisation("en", "EWDemoaction", "Have all the cheesecake in the world!");
            registry.RegisterGlossary(glossary);
            EWandererDemoAction.glossary_item = glossary.Head;
        }

        public void LoadManifest(IArtifactRegistry registry)
        {
        }

        public void LoadManifest(IStatusRegistry statusRegistry)
        {
        }

        public void LoadManifest(ICustomEventHub eventHub)
        {
        }
    }
}