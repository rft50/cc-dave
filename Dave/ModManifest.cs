using CobaltCoreModding.Definitions;
using CobaltCoreModding.Definitions.ExternalItems;
using CobaltCoreModding.Definitions.ModContactPoints;
using CobaltCoreModding.Definitions.ModManifests;
using Dave.Actions;
using Dave.Artifacts;
using Dave.Cards;
using HarmonyLib;
using Microsoft.Extensions.Logging;

namespace Dave
{
    public partial class ModManifest : IModManifest, ISpriteManifest, IAnimationManifest, IDeckManifest, ICardManifest, ICardOverwriteManifest, ICharacterManifest, IGlossaryManifest, IArtifactManifest, IStatusManifest, ICustomEventManifest
    {
        public static ExternalStatus? red_rigging;
        public static ExternalStatus? black_rigging;
        public static ExternalStatus? red_bias;
        public static ExternalStatus? black_bias;
        public static ExternalDeck? dave_deck;
        public static ICustomEventHub EventHub;
        private ExternalSprite? card_art_sprite;
        private ExternalSprite? character_frame_sprite;
        private ExternalAnimation? default_animation;
        private ExternalAnimation? squint_animation;
        private ExternalSprite? dave_art;
        private ExternalSprite? dave_border;
        private ExternalAnimation? mini_animation;
        private ExternalAnimation? game_over_animation;
        private ExternalSprite? mini_dave_sprite;
        private ExternalSprite? game_over_sprite;
        private ExternalSprite? random_move_foe_sprite;
        private ExternalSprite? shield_hurt_sprite;
        private ExternalSprite? red_black_sprite;
        private ExternalSprite? red_sprite;
        private ExternalSprite? black_sprite;
        private ExternalSprite? red_chip_sprite;
        private ExternalSprite? black_chip_sprite;
        private ExternalSprite? red_clover_sprite;
        private ExternalSprite? black_clover_sprite;
        private Dictionary<string, ExternalSprite[]> animations = new();
        private ExternalSprite? artifact_chip_red;
        private ExternalSprite? artifact_chip_black;
        private ExternalSprite? artifact_chip_both;
        private ExternalSprite? artifact_chip_neutral;
        private ExternalSprite? artifact_drive_refund_on;
        private ExternalSprite? artifact_drive_refund_off;
        private ExternalSprite? artifact_ashtray;
        private ExternalSprite? artifact_roulette;
        private ExternalSprite? artifact_underdrive_generator;
        private ExternalSprite? artifact_rigged_dice;
        private Dictionary<string, ExternalSprite> cards = new();
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
                card_art_sprite = new ExternalSprite("rft.Dave.DaveCardFrame", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(card_art_sprite))
                    throw new Exception("Cannot register frame_dave.png.");
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("char_frame_dave.png"));
                character_frame_sprite = new ExternalSprite("rft.Dave.DaveFrame", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(character_frame_sprite))
                    throw new Exception("Cannot register char_frame_dave.png.");
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", "Animation", Path.GetFileName("DaveMini.png"));
                mini_dave_sprite = new ExternalSprite("rft.Dave.Anim.mini", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(mini_dave_sprite))
                    throw new Exception("Cannot register animation DaveMini.png.");
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", "Animation", Path.GetFileName("DaveGameOver.png"));
                game_over_sprite = new ExternalSprite("rft.Dave.Anim.gameover", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(game_over_sprite))
                    throw new Exception("Cannot register animation DaveGameOver.png.");
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("random_move_foe.png"));
                random_move_foe_sprite = new ExternalSprite("rft.Dave.random_move_foe", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(random_move_foe_sprite))
                    throw new Exception("Cannot register random_move_foe.png.");
                RandomMoveFoeAction.spr = (Spr)(random_move_foe_sprite.Id ?? throw new NullReferenceException());
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("shield_hurt.png"));
                shield_hurt_sprite = new ExternalSprite("rft.Dave.shield_hurt", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(shield_hurt_sprite))
                    throw new Exception("Cannot register shield_hurt.png.");
                ShieldHurtAction.spr = (Spr)(shield_hurt_sprite.Id ?? throw new NullReferenceException());
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
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("black.png"));
                black_sprite = new ExternalSprite("rft.Dave.black", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(black_sprite))
                    throw new Exception("Cannot register black.png.");
                CardRenderPatch.black = (Spr) (black_sprite.Id ?? throw new NullReferenceException());
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("red_chip.png"));
                red_chip_sprite = new ExternalSprite("rft.Dave.red_chip", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(red_chip_sprite))
                    throw new Exception("Cannot register red_chip.png.");
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("black_chip.png"));
                black_chip_sprite = new ExternalSprite("rft.Dave.black_chip", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(black_chip_sprite))
                    throw new Exception("Cannot register black_chip.png.");
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("red_clover.png"));
                red_clover_sprite = new ExternalSprite("rft.Dave.red_clover", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(red_clover_sprite))
                    throw new Exception("Cannot register red_clover.png.");
                BiasStatusAction.spr_red = (Spr) (red_clover_sprite.Id ?? throw new NullReferenceException());
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("black_clover.png"));
                black_clover_sprite = new ExternalSprite("rft.Dave.black_clover", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(black_clover_sprite))
                    throw new Exception("Cannot register black_clover.png.");
                BiasStatusAction.spr_black = (Spr) (black_clover_sprite.Id ?? throw new NullReferenceException());
            }
            {
                var groups = new[]
                {
                    "Neutral",
                    "Squint"
                };
                foreach (var g in groups)
                {
                    var sprites = new ExternalSprite[4];

                    for (var i = 0; i < sprites.Length; i++)
                    {
                        var path = Path.Combine(ModRootFolder.FullName, "Sprites", "Animation",
                            Path.GetFileName("Dave" + g + i + ".png"));
                        sprites[i] = new ExternalSprite("rft.Dave.Anim." + g + i, new FileInfo(path));
                        if (!spriteRegistry.RegisterArt(sprites[i]))
                            throw new Exception("Cannot register animation " + path);
                    }

                    animations[g] = sprites;
                }
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", "Artifact", Path.GetFileName("Chip_red.png"));
                artifact_chip_red = new ExternalSprite("rft.Dave.Artifact.Chip_red", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(artifact_chip_red))
                    throw new Exception("Cannot register Chip_red.png.");
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", "Artifact", Path.GetFileName("Chip_black.png"));
                artifact_chip_black = new ExternalSprite("rft.Dave.Artifact.Chip_black", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(artifact_chip_black))
                    throw new Exception("Cannot register Chip_black.png.");
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", "Artifact", Path.GetFileName("Chip_both.png"));
                artifact_chip_both = new ExternalSprite("rft.Dave.Artifact.Chip_both", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(artifact_chip_both))
                    throw new Exception("Cannot register Chip_both.png.");
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", "Artifact", Path.GetFileName("Chip_neutral.png"));
                artifact_chip_neutral = new ExternalSprite("rft.Dave.Artifact.Chip_neutral", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(artifact_chip_neutral))
                    throw new Exception("Cannot register Chip_neutral.png.");
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", "Artifact", Path.GetFileName("DriveRefund.png"));
                artifact_drive_refund_on = new ExternalSprite("rft.Dave.Artifact.DriveRefund", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(artifact_drive_refund_on))
                    throw new Exception("Cannot register DriveRefund.png.");
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", "Artifact", Path.GetFileName("DriveRefund_off.png"));
                artifact_drive_refund_off = new ExternalSprite("rft.Dave.Artifact.DriveRefund_off", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(artifact_drive_refund_off))
                    throw new Exception("Cannot register DriveRefund_off.png.");
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", "Artifact", Path.GetFileName("Ashtray.png"));
                artifact_ashtray = new ExternalSprite("rft.Dave.Artifact.Ashtray", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(artifact_ashtray))
                    throw new Exception("Cannot register Ashtray.png.");
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", "Artifact", Path.GetFileName("Roulette.png"));
                artifact_roulette = new ExternalSprite("rft.Dave.Artifact.Roulette", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(artifact_roulette))
                    throw new Exception("Cannot register Roulette.png.");
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", "Artifact", Path.GetFileName("RiggedDice.png"));
                artifact_rigged_dice = new ExternalSprite("rft.Dave.Artifact.RiggedDice", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(artifact_rigged_dice))
                    throw new Exception("Cannot register RiggedDice.png.");
            }
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", "Artifact", Path.GetFileName("UnderdriveGenerator.png"));
                artifact_underdrive_generator = new ExternalSprite("rft.Dave.Artifact.UnderdriveGenerator", new FileInfo(path));
                if (!spriteRegistry.RegisterArt(artifact_underdrive_generator))
                    throw new Exception("Cannot register UnderdriveGenerator.png.");
            }
            {
                var cards = new[]
                {
                    "common",
                    "red",
                    "black"
                };

                foreach (var c in cards)
                {
                    var path = Path.Combine(ModRootFolder.FullName, "Sprites", "Card", Path.GetFileName("dave_" + c + ".png"));
                    var sprite = new ExternalSprite("rft.Dave.Card." + c, new FileInfo(path));
                    if (!spriteRegistry.RegisterArt(sprite))
                        throw new Exception("Cannot register cardback " + path);

                    this.cards[c] = sprite;
                }
            }
        }

        public void LoadManifest(IAnimationRegistry registry)
        {
            default_animation = new ExternalAnimation("rft.Dave.Dave.neutral", dave_deck ?? throw new NullReferenceException(), "neutral", false, new ExternalSprite[] {
                animations["Neutral"][0],
                animations["Neutral"][1],
                animations["Neutral"][2],
                animations["Neutral"][3]
            });
            
            registry.RegisterAnimation(default_animation);
            
            squint_animation = new ExternalAnimation("rft.Dave.Dave.squint", dave_deck ?? throw new NullReferenceException(), "squint", false, new ExternalSprite[] {
                animations["Squint"][0],
                animations["Squint"][1],
                animations["Squint"][2],
                animations["Squint"][3]
            });
            
            registry.RegisterAnimation(squint_animation);
            
            if (mini_dave_sprite == null)
                throw new Exception();
            
            mini_animation = new ExternalAnimation("rft.Dave.Dave.mini", dave_deck, "mini", false, new ExternalSprite[] { mini_dave_sprite });
            
            registry.RegisterAnimation(mini_animation);
            
            if (game_over_sprite == null)
                throw new Exception();

            game_over_animation = new ExternalAnimation("rft.Dave.Dave.gameover", dave_deck, "gameover", false, new ExternalSprite[] { game_over_sprite });

            registry.RegisterAnimation(game_over_animation);
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
            LuckyEscapeCard.card_sprite = (Spr) cards["common"].Id!;
            
            var luckyEscape = new ExternalCard("rft.Dave.LuckyEscapeCard", typeof(LuckyEscapeCard), cards["common"], dave_deck);
            luckyEscape.AddLocalisation("Lucky Escape");
            registry.RegisterCard(luckyEscape);

            RiggingShotCard.card_sprite = (Spr) cards["common"].Id!;
            
            var riggingShot = new ExternalCard("rft.Dave.RiggingShotCard", typeof(RiggingShotCard), cards["common"], dave_deck);
            riggingShot.AddLocalisation("Rigging Shot");
            registry.RegisterCard(riggingShot);

            // common
            WildStepCard.card_sprite = (Spr) cards["common"].Id!;
            
            var wildStep = new ExternalCard("rft.Dave.WildStepCard", typeof(WildStepCard), cards["common"], dave_deck);
            wildStep.AddLocalisation("Wild Step");
            registry.RegisterCard(wildStep);

            WildShotCard.card_sprite = (Spr) cards["common"].Id!;
            
            var wildShot = new ExternalCard("rft.Dave.WildShotCard", typeof(WildShotCard), cards["common"], dave_deck);
            wildShot.AddLocalisation("Wild Shot");
            registry.RegisterCard(wildShot);

            WindupCard.card_sprite = (Spr) cards["common"].Id!;
            
            var windup = new ExternalCard("rft.Dave.WindupCard", typeof(WindupCard), cards["common"], dave_deck);
            windup.AddLocalisation("Windup");
            registry.RegisterCard(windup);

            PrimedShotCard.card_sprite = (Spr) cards["common"].Id!;
            
            var primedShot = new ExternalCard("rft.Dave.PrimedShotCard", typeof(PrimedShotCard), cards["common"], dave_deck);
            primedShot.AddLocalisation("Primed Shot");
            registry.RegisterCard(primedShot);

            PinchShotCard.card_sprite = (Spr) cards["common"].Id!;
            
            var pinchShot = new ExternalCard("rft.Dave.PinchShotCard", typeof(PinchShotCard), cards["common"], dave_deck);
            pinchShot.AddLocalisation("Pinch Shot");
            registry.RegisterCard(pinchShot);

            WildWallCard.card_sprite = (Spr) cards["common"].Id!;
            
            var wildWall = new ExternalCard("rft.Dave.WildWallCard", typeof(WildWallCard), cards["common"], dave_deck);
            wildWall.AddLocalisation("Wild Wall");
            registry.RegisterCard(wildWall);

            FoldCard.card_sprite = (Spr) cards["common"].Id!;
            
            var fold = new ExternalCard("rft.Dave.FoldCard", typeof(FoldCard), cards["common"], dave_deck);
            fold.AddLocalisation("Fold");
            registry.RegisterCard(fold);
            
            // uncommon
            WildBarrageCard.card_sprite = (Spr) cards["common"].Id!;
            
            var wildBarrage = new ExternalCard("rft.Dave.WildBarrageCard", typeof(WildBarrageCard), cards["common"], dave_deck);
            wildBarrage.AddLocalisation("Wild Barrage");
            registry.RegisterCard(wildBarrage);
            
            RiggingCard.card_sprite = (Spr) cards["common"].Id!;
            RiggingCard.red_sprite = (Spr)cards["red"].Id!;
            RiggingCard.black_sprite = (Spr)cards["black"].Id!;
            
            var rigging = new ExternalCard("rft.Dave.RiggingCard", typeof(RiggingCard), cards["common"], dave_deck);
            rigging.AddLocalisation("Rigging");
            registry.RegisterCard(rigging);
            
            RaiseCard.card_sprite = (Spr) cards["common"].Id!;
            
            var raise = new ExternalCard("rft.Dave.Raise", typeof(RaiseCard), cards["common"], dave_deck);
            raise.AddLocalisation("Raise");
            registry.RegisterCard(raise);

            InvestmentCard.card_sprite = (Spr) cards["common"].Id!;
            
            var investment = new ExternalCard("rft.Dave.Investment", typeof(InvestmentCard), cards["common"], dave_deck);
            investment.AddLocalisation("Investment");
            registry.RegisterCard(investment);

            LowballCard.card_sprite = (Spr) cards["common"].Id!;
            
            var lowball = new ExternalCard("rft.Dave.Lowball", typeof(LowballCard), cards["common"], dave_deck);
            lowball.AddLocalisation("Lowball");
            registry.RegisterCard(lowball);

            LuckyShotCard.card_sprite = (Spr) cards["common"].Id!;
            
            var luckyShot = new ExternalCard("rft.Dave.LuckyShotCard", typeof(LuckyShotCard), cards["common"], dave_deck);
            luckyShot.AddLocalisation("Lucky Shot");
            registry.RegisterCard(luckyShot);

            SeeingRedCard.card_sprite = (Spr) cards["common"].Id!;
            
            var seeingRed = new ExternalCard("rft.Dave.SeeingRedCard", typeof(SeeingRedCard), cards["common"], dave_deck);
            seeingRed.AddLocalisation("Seeing Red");
            registry.RegisterCard(seeingRed);
            
            // rare
            LoadedDiceCard.red_sprite = (Spr)cards["red"].Id!;
            LoadedDiceCard.black_sprite = (Spr)cards["black"].Id!;
            
            var loadedDice = new ExternalCard("rft.Dave.LoadedDiceCard", typeof(LoadedDiceCard), cards["common"], dave_deck);
            loadedDice.AddLocalisation("Loaded Dice");
            registry.RegisterCard(loadedDice);
            
            AllInCard.card_sprite = (Spr) cards["common"].Id!;
            
            var allIn = new ExternalCard("rft.Dave.AllInCard", typeof(AllInCard), cards["common"], dave_deck);
            allIn.AddLocalisation("All In");
            registry.RegisterCard(allIn);
            
            EvenBetCard.card_sprite = (Spr) cards["common"].Id!;
            
            var evenBet = new ExternalCard("rft.Dave.EvenBetCard", typeof(EvenBetCard), cards["common"], dave_deck);
            evenBet.AddLocalisation("Even Bet");
            registry.RegisterCard(evenBet);
            
            AllBetsAreOffCard.card_sprite = (Spr) cards["common"].Id!;
            
            var allBetsAreOff = new ExternalCard("rft.Dave.AllBetsAreOffCard", typeof(AllBetsAreOffCard), cards["common"], dave_deck);
            allBetsAreOff.AddLocalisation("All Bets Are Off");
            registry.RegisterCard(allBetsAreOff);
            
            DrawnGambitCard.card_sprite = (Spr) cards["common"].Id!;
            
            var drawnGambit = new ExternalCard("rft.Dave.DrawnGambitCard", typeof(DrawnGambitCard), cards["common"], dave_deck);
            drawnGambit.AddLocalisation("Drawn Gambit");
            registry.RegisterCard(drawnGambit);
            
            // SPECIAL
            PerfectOddsCard.red_sprite = (Spr)cards["red"].Id!;
            PerfectOddsCard.black_sprite = (Spr)cards["black"].Id!;
            
            var perfectOdds = new ExternalCard("rft.Dave.PerfectOddsCard", typeof(PerfectOddsCard), cards["common"], dave_deck);
            perfectOdds.AddLocalisation("Perfect Odds");
            registry.RegisterCard(perfectOdds);
        }

        public void LoadManifest(ICardOverwriteRegistry registry)
        {
        }

        public void LoadManifest(ICharacterRegistry registry)
        {
            var dave_spr = character_frame_sprite;
            
            var start_cards = new[] { typeof(LuckyEscapeCard), typeof(RiggingShotCard) };
            var dave_char = new ExternalCharacter("rft.Dave.DaveChar", dave_deck ?? throw new NullReferenceException(), dave_spr, start_cards, new[] { typeof(Chip) }, default_animation ?? throw new NullReferenceException(), mini_animation ?? throw new NullReferenceException());
            dave_char.AddNameLocalisation("Dave");
            dave_char.AddDescLocalisation("<c=159D3E>DAVE</c>\nA gambler. His cards have random effects and allow him to rig the odds.");
            registry.RegisterCharacter(dave_char);
        }

        public void LoadManifest(IGlossaryRegisty registry)
        {
            var randomMoveFoeGlossary = new ExternalGlossary("rft.Dave.RandomMoveFoe.Glossary", "randommovefoe", false,
                ExternalGlossary.GlossayType.action, random_move_foe_sprite);
            randomMoveFoeGlossary.AddLocalisation("en", "random foe move", "Instantly move the opponent {0} spaces in a random direction.");
            registry.RegisterGlossary(randomMoveFoeGlossary);
            RandomMoveFoeAction.glossary_item = randomMoveFoeGlossary.Head;

            var shieldHurtGlossary = new ExternalGlossary("rft.Dave.ShieldHurt.Glossary", "shieldhurt", false,
                ExternalGlossary.GlossayType.action, shield_hurt_sprite);
            shieldHurtGlossary.AddLocalisation("en", "shield hurt", "Take {0} damage, hitting shields first.");
            registry.RegisterGlossary(shieldHurtGlossary);
            ShieldHurtAction.glossary_item = shieldHurtGlossary.Head;

            var redBlackGlossary = new ExternalGlossary("rft.Dave.RedBlack.Glossary", "RedBlack", false,
                ExternalGlossary.GlossayType.action, red_black_sprite);
            redBlackGlossary.AddLocalisation("en", "red/black", "When played, either performs the Red or Black actions.");
            registry.RegisterGlossary(redBlackGlossary);
            RandomChoiceActionFactory.glossary_item = redBlackGlossary.Head;
            
            var redBiasGlossary = new ExternalGlossary("rft.Dave.RedBias.Glossary", "RedBias", false,
                ExternalGlossary.GlossayType.action, red_clover_sprite);
            redBiasGlossary.AddLocalisation("en", "red bias", "Each stack of this makes Red/Black actions 10% more likely to roll Red.");
            registry.RegisterGlossary(redBiasGlossary);
            BiasStatusAction.blue_glossary_item = redBiasGlossary.Head;
            
            var blackBiasGlossary = new ExternalGlossary("rft.Dave.BlackBias.Glossary", "BlackBias", false,
                ExternalGlossary.GlossayType.action, black_clover_sprite);
            blackBiasGlossary.AddLocalisation("en", "black bias", "Each stack of this makes Red/Black actions 10% more likely to roll Black.");
            registry.RegisterGlossary(blackBiasGlossary);
            BiasStatusAction.orange_glossary_item = blackBiasGlossary.Head;
        }

        public void LoadManifest(IStatusRegistry statusRegistry)
        {
            red_rigging = new ExternalStatus("rft.Dave.RedRiggingStatus", true, System.Drawing.Color.Red, null, red_chip_sprite ?? throw new Exception("missing sprite"), false);
            statusRegistry.RegisterStatus(red_rigging);
            red_rigging.AddLocalisation("Red Rigging", "The next {0} cards with Red/Black actions will go Red.");
            
            black_rigging = new ExternalStatus("rft.Dave.BlackRiggingStatus", true, System.Drawing.Color.Black, null, black_chip_sprite ?? throw new Exception("missing sprite"), false);
            statusRegistry.RegisterStatus(black_rigging);
            black_rigging.AddLocalisation("Black Rigging", "The next {0} cards with Red/Black actions will go Black.");
            
            red_bias = new ExternalStatus("rft.Dave.RedBiasStatus", true, System.Drawing.Color.Red, null, red_clover_sprite ?? throw new Exception("missing sprite"), false);
            statusRegistry.RegisterStatus(red_bias);
            red_bias.AddLocalisation("Red Bias", "Each stack of this makes Red/Black actions 10% more likely to roll Red.");
            
            black_bias = new ExternalStatus("rft.Dave.BlackBiasStatus", true, System.Drawing.Color.Black, null, black_clover_sprite ?? throw new Exception("missing sprite"), false);
            statusRegistry.RegisterStatus(black_bias);
            black_bias.AddLocalisation("Black Bias", "Each stack of this makes Red/Black actions 10% more likely to roll Black.");
        }
    }
}