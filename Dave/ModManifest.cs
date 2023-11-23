using CobaltCoreModding.Definitions;
using CobaltCoreModding.Definitions.ExternalItems;
using CobaltCoreModding.Definitions.ModContactPoints;
using CobaltCoreModding.Definitions.ModManifests;
using Dave.Actions;
using Dave.Cards;
using HarmonyLib;
using Microsoft.Extensions.Logging;

namespace Dave
{
    public class ModManifest : IModManifest, ISpriteManifest, IAnimationManifest, IDeckManifest, ICardManifest, ICardOverwriteManifest, ICharacterManifest, IGlossaryManifest, IArtifactManifest, IStatusManifest, ICustomEventManifest
    {
        public static ExternalStatus? red_rigging;
        public static ExternalStatus? black_rigging;
        public static ExternalStatus? red_bias;
        public static ExternalStatus? black_bias;
        public static ExternalDeck? dave_deck;
        private ExternalSprite? card_art_sprite;
        private ExternalAnimation? default_animation;
        private ExternalSprite? dave_art;
        private ExternalSprite? dave_border;
        private ExternalAnimation? mini_animation;
        private ExternalSprite? mini_dave_sprite;
        private ExternalSprite? random_move_foe_sprite;
        private ExternalSprite? red_black_sprite;
        private ExternalSprite? red_sprite;
        private ExternalSprite? black_sprite;
        private IEnumerable<DependencyEntry> _dependencies => Array.Empty<DependencyEntry>();
        public IEnumerable<string> Dependencies => new string[0];
        public ILogger? Logger { get; set; }
        public DirectoryInfo? ModRootFolder { get; set; }

        IEnumerable<DependencyEntry> IManifest.Dependencies => _dependencies;

        public DirectoryInfo? GameRootFolder { get; set; }
        public string Name => "Dave";

        public void BootMod(IModLoaderContact contact)
        {
            var harmony = new Harmony("Dave");
            harmony.PatchAll();
        }

        public void LoadManifest(ISpriteRegistry spriteRegistry)
        {
            if (ModRootFolder == null)
                throw new Exception("No root folder set!");

            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("frame_dave.png"));
                card_art_sprite = new ExternalSprite("rft.Dave.DaveFrame", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(card_art_sprite))
                    throw new Exception("Cannot register frame_dave.png.");
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("dracula_mini_0.png"));
                mini_dave_sprite = new ExternalSprite("EWanderer.DemoMod.dracular.mini", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(mini_dave_sprite))
                    throw new Exception("Cannot register dracula_mini_0.png.");
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("random_move_foe.png"));
                random_move_foe_sprite = new ExternalSprite("rft.Dave.random_move_foe", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(random_move_foe_sprite))
                    throw new Exception("Cannot register random_move_foe.png.");
                RandomMoveFoeAction.spr = (Spr)(random_move_foe_sprite.Id ?? throw new NullReferenceException());
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("red_black.png"));
                red_black_sprite = new ExternalSprite("rft.Dave.red_black", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(red_black_sprite))
                    throw new Exception("Cannot register red_black.png.");
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("red.png"));
                red_sprite = new ExternalSprite("rft.Dave.red", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(red_sprite))
                    throw new Exception("Cannot register red.png.");
                CardRenderPatch.red = (Spr) (red_sprite.Id ?? throw new NullReferenceException());
                BiasStatusAction.spr_red = (Spr) red_sprite.Id;
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("black.png"));
                black_sprite = new ExternalSprite("rft.Dave.black", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(black_sprite))
                    throw new Exception("Cannot register black.png.");
                CardRenderPatch.black = (Spr) (black_sprite.Id ?? throw new NullReferenceException());
                BiasStatusAction.spr_black = (Spr) black_sprite.Id;
            }
        }

        public void LoadManifest(IAnimationRegistry registry)
        {
            default_animation = new ExternalAnimation("rft.Dave.Dave.neutral", dave_deck ?? throw new NullReferenceException(), "neutral", false, new ExternalSprite[] {
                ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_0),
                ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_1),
                ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_2),
                ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_3),
                ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_4),
            });
            
            registry.RegisterAnimation(default_animation);
            if (mini_dave_sprite == null)
                throw new Exception();
            
            mini_animation = new ExternalAnimation("rft.Dave.Dave.mini", dave_deck, "mini", false, new ExternalSprite[] { mini_dave_sprite });
            
            registry.RegisterAnimation(mini_animation);
        }

        public void LoadManifest(IDeckRegistry registry)
        {
             dave_art = ExternalSprite.GetRaw((int)Spr.cards_colorless);
             dave_border = card_art_sprite;
            
             dave_deck = new ExternalDeck("rft.Dave.DaveDeck", System.Drawing.Color.PaleGreen, System.Drawing.Color.Black, dave_art ?? throw new NullReferenceException(), dave_border ?? throw new NullReferenceException(), null);
            
             if (!registry.RegisterDeck(dave_deck))
                 return;
        }

        public void LoadManifest(ICardRegistry registry)
        {
            if (card_art_sprite == null)
                return;

            // starter
            var wildStep = new ExternalCard("rft.Dave.WildStepCard", typeof(WildStepCard), card_art_sprite, dave_deck);
            wildStep.AddLocalisation("Wild Step");
            registry.RegisterCard(wildStep);

            var wildShot = new ExternalCard("rft.Dave.WildShotCard", typeof(WildShotCard), card_art_sprite, dave_deck);
            wildShot.AddLocalisation("Wild Shot");
            registry.RegisterCard(wildShot);

            // common
            var raise = new ExternalCard("rft.Dave.Raise", typeof(RaiseCard), card_art_sprite, dave_deck);
            raise.AddLocalisation("Raise");
            registry.RegisterCard(raise);
            
            // uncommon
            var wildBarrage = new ExternalCard("rft.Dave.WildBarrageCard", typeof(WildBarrageCard), card_art_sprite, dave_deck);
            wildBarrage.AddLocalisation("Wild Barrage");
            registry.RegisterCard(wildBarrage);
            
            var rigging = new ExternalCard("rft.Dave.RiggingCard", typeof(RiggingCard), card_art_sprite, dave_deck);
            rigging.AddLocalisation("Rigging");
            registry.RegisterCard(rigging);
            
            // rare
            var loadedDice = new ExternalCard("rft.Dave.LoadedDiceCard", typeof(LoadedDiceCard), card_art_sprite, dave_deck);
            loadedDice.AddLocalisation("Loaded Dice");
            registry.RegisterCard(loadedDice);
            
            var allIn = new ExternalCard("rft.Dave.AllInCard", typeof(AllInCard), card_art_sprite, dave_deck);
            allIn.AddLocalisation("All In");
            registry.RegisterCard(allIn);
            
            var evenBet = new ExternalCard("rft.Dave.EvenBetCard", typeof(EvenBetCard), card_art_sprite, dave_deck);
            evenBet.AddLocalisation("Even Bet");
            registry.RegisterCard(evenBet);
            
            var allBetsAreOff = new ExternalCard("rft.Dave.AllBetsAreOffCard", typeof(AllBetsAreOffCard), card_art_sprite, dave_deck);
            allBetsAreOff.AddLocalisation("All Bets Are Off");
            registry.RegisterCard(allBetsAreOff);
        }

        public void LoadManifest(ICardOverwriteRegistry registry)
        {
        }

        public void LoadManifest(ICharacterRegistry registry)
        {
            var dave_spr = ExternalSprite.GetRaw((int)Spr.panels_char_colorless);
            
            var start_cards = new Type[] { typeof(WildStepCard), typeof(WildShotCard) };
            var dave_char = new ExternalCharacter("rft.Dave.DaveChar", dave_deck ?? throw new NullReferenceException(), dave_spr, start_cards, new Type[0], default_animation ?? throw new NullReferenceException(), mini_animation ?? throw new NullReferenceException());
            dave_char.AddNameLocalisation("Dave");
            dave_char.AddDescLocalisation("A gambler.");
            registry.RegisterCharacter(dave_char);
        }

        public void LoadManifest(IGlossaryRegisty registry)
        {
            var randomMoveFoeGlossary = new ExternalGlossary("rft.Dave.RandomMoveFoe.Glossary", "randommovefoe", false,
                ExternalGlossary.GlossayType.action, random_move_foe_sprite);
            randomMoveFoeGlossary.AddLocalisation("en", "random foe move", "Instantly move the opponent {0} spaces in a random direction.");
            registry.RegisterGlossary(randomMoveFoeGlossary);
            RandomMoveFoeAction.glossary_item = randomMoveFoeGlossary.Head;

            var redBlackGlossary = new ExternalGlossary("rft.Dave.RedBlack.Glossary", "RedBlack", false,
                ExternalGlossary.GlossayType.action, red_black_sprite);
            redBlackGlossary.AddLocalisation("en", "red/black", "When played, either performs the Red or Black actions.");
            registry.RegisterGlossary(redBlackGlossary);
            RandomChoiceActionFactory.glossary_item = redBlackGlossary.Head;
            
            var redBiasGlossary = new ExternalGlossary("rft.Dave.RedBias.Glossary", "RedBias", false,
                ExternalGlossary.GlossayType.action, red_sprite);
            redBiasGlossary.AddLocalisation("en", "red bias", "Each stack of this makes Red/Black actions 10% more likely to roll Red.");
            registry.RegisterGlossary(redBiasGlossary);
            BiasStatusAction.blue_glossary_item = redBiasGlossary.Head;
            
            var blackBiasGlossary = new ExternalGlossary("rft.Dave.BlackBias.Glossary", "BlackBias", false,
                ExternalGlossary.GlossayType.action, black_sprite);
            blackBiasGlossary.AddLocalisation("en", "black bias", "Each stack of this makes Red/Black actions 10% more likely to roll Black.");
            registry.RegisterGlossary(blackBiasGlossary);
            BiasStatusAction.orange_glossary_item = blackBiasGlossary.Head;
        }

        public void LoadManifest(IArtifactRegistry registry)
        {
        }

        public void LoadManifest(IStatusRegistry statusRegistry)
        {
            red_rigging = new ExternalStatus("rft.Dave.RedRiggingStatus", true, System.Drawing.Color.Red, null, red_sprite ?? throw new Exception("missing sprite"), false);
            statusRegistry.RegisterStatus(red_rigging);
            red_rigging.AddLocalisation("Red Rigging", "The next {0} cards with Red/Black actions will go Red.");
            
            black_rigging = new ExternalStatus("rft.Dave.BlackRiggingStatus", true, System.Drawing.Color.Black, null, black_sprite ?? throw new Exception("missing sprite"), false);
            statusRegistry.RegisterStatus(black_rigging);
            black_rigging.AddLocalisation("Black Rigging", "The next {0} cards with Red/Black actions will go Black.");
            
            red_bias = new ExternalStatus("rft.Dave.RedBiasStatus", true, System.Drawing.Color.Red, null, red_sprite ?? throw new Exception("missing sprite"), false);
            statusRegistry.RegisterStatus(red_bias);
            red_bias.AddLocalisation("Red Bias", "Each stack of this makes Red/Black actions 10% more likely to roll Red.");
            
            black_bias = new ExternalStatus("rft.Dave.BlackBiasStatus", true, System.Drawing.Color.Black, null, black_sprite ?? throw new Exception("missing sprite"), false);
            statusRegistry.RegisterStatus(black_bias);
            black_bias.AddLocalisation("Black Bias", "Each stack of this makes Red/Black actions 10% more likely to roll Black.");
        }

        public void LoadManifest(ICustomEventHub eventHub)
        {
        }
    }
}