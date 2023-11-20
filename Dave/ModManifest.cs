using CobaltCoreModding.Definitions.ExternalItems;
using CobaltCoreModding.Definitions.ModContactPoints;
using CobaltCoreModding.Definitions.ModManifests;
using CobaltCoreModding.Definitions.OverwriteItems;
using Dave.Actions;
using Dave.Cards;
using HarmonyLib;

namespace Dave
{
    // IDBManifest
    public class ModManifest : IModManifest, ISpriteManifest, IAnimationManifest, IDeckManifest, ICardManifest, ICardOverwriteManifest, ICharacterManifest, IGlossaryManifest, IArtifactManifest, IStatusManifest, ICustomEventManifest
    {
        public static ExternalStatus? demo_status;
        internal static ICustomEventHub? EventHub;
        internal static int x = 0;
        private ExternalSprite? card_art_sprite;
        private ExternalAnimation? default_animation;
        private ExternalSprite? demo_status_sprite;
        private ExternalDeck? dracula_deck;
        private ExternalSprite? dracular_art;
        private ExternalSprite? dracular_border;
        private ExternalAnimation? mini_animation;
        private ExternalSprite? mini_dracula_sprite;
        private ExternalSprite? pinker_per_border_over_sprite;
        private ExternalSprite? random_move_foe_sprite;
        private ExternalSprite? blue_orange_sprite;
        private ExternalSprite? blue_sprite;
        private ExternalSprite? orange_sprite;
        public IEnumerable<string> Dependencies => new string[0];
        public DirectoryInfo? ModRootFolder { get; set; }
        public DirectoryInfo? GameRootFolder { get; set; }
        public string Name => "Dave";

        public void BootMod(IModLoaderContact contact)
        {
            var harmony = new Harmony("Dave");
            harmony.PatchAll();
        }

        public void LoadManifest(IArtRegistry artRegistry)
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

            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("random_move_foe.png"));
                random_move_foe_sprite = new ExternalSprite("rft.Dave.random_move_foe", new FileInfo(path));
                if (!artRegistry.RegisterArt(random_move_foe_sprite))
                    throw new Exception("Cannot register sprite.");
                RandomMoveFoeAction.spr = (Spr)(random_move_foe_sprite.Id ?? throw new NullReferenceException());
            }

            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("blue_orange.png"));
                blue_orange_sprite = new ExternalSprite("rft.Dave.blue_orange", new FileInfo(path));
                if (!artRegistry.RegisterArt(blue_orange_sprite))
                    throw new Exception("Cannot register sprite.");
            }

            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("blue.png"));
                blue_sprite = new ExternalSprite("rft.Dave.blue", new FileInfo(path));
                if (!artRegistry.RegisterArt(blue_sprite))
                    throw new Exception("Cannot register sprite.");
                CardRenderPatch.blue = (Spr) (blue_sprite.Id ?? throw new NullReferenceException());
            }

            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("orange.png"));
                orange_sprite = new ExternalSprite("rft.Dave.orange", new FileInfo(path));
                if (!artRegistry.RegisterArt(orange_sprite))
                    throw new Exception("Cannot register sprite.");
                CardRenderPatch.orange = (Spr) (orange_sprite.Id ?? throw new NullReferenceException());
            }
        }

        public void LoadManifest(IDbRegistry dbRegistry)
        {
        }

        public void LoadManifest(IAnimationRegistry registry)
        {
            // default_animation = new ExternalAnimation("ewanderer.demomod.dracula.neutral", dracula_deck ?? throw new NullReferenceException(), "neutral", false, new ExternalSprite[] {
            //     ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_0),
            //     ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_1),
            //     ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_2),
            //     ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_3),
            //     ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_4),
            // });
            //
            // registry.RegisterAnimation(default_animation);
            // if (mini_dracula_sprite == null)
            //     throw new Exception();
            //
            // mini_animation = new ExternalAnimation("ewanderer.demomod.dracula.mini", dracula_deck, "mini", false, new ExternalSprite[] { mini_dracula_sprite });
            //
            // registry.RegisterAnimation(mini_animation);
        }

        public void LoadManifest(IDeckRegistry registry)
        {
            //make peri deck mod
            // var art_default = ExternalSprite.GetRaw((int)Spr.cards_WaveBeam);
            // var border = ExternalSprite.GetRaw((int)Spr.cardShared_border_ephemeral);
            //
            // var pinker_peri = new ExternalDeck("Ewanderer.DemoMod.PinkerPeri", System.Drawing.Color.Brown, System.Drawing.Color.Yellow, art_default, border, pinker_per_border_over_sprite);
            // registry.RegisterDeck(pinker_peri, (int)Deck.peri);
            //
            // dracular_art = ExternalSprite.GetRaw((int)Spr.cards_colorless);
            // dracular_border = ExternalSprite.GetRaw((int)Spr.cardShared_border_dracula);
            //
            // dracula_deck = new ExternalDeck("EWanderer.Demomod.DraculaDeck", System.Drawing.Color.Crimson, System.Drawing.Color.Purple, dracular_art ?? throw new NullReferenceException(), dracular_border ?? throw new NullReferenceException(), null);
            //
            // if (!registry.RegisterDeck(dracula_deck))
            //     return;
        }

        public void LoadManifest(ICardRegistry registry)
        {
            if (card_art_sprite == null)
                return;
            //make card meta data
            var card = new ExternalCard("Ewanderer.DemoMod.DemoCard", typeof(EWandererDemoCard), card_art_sprite, null);
            //add card name in english
            card.AddLocalisation("Schwarzmagier");
            //register card in the db extender.
            registry.RegisterCard(card);

            var wildStep = new ExternalCard("rft.Dave.WildStepCard", typeof(WildStepCard), card_art_sprite, null);
            wildStep.AddLocalisation("Wild Step");
            registry.RegisterCard(wildStep);

            var wildShot = new ExternalCard("rft.Dave.WildShotCard", typeof(WildShotCard), card_art_sprite, null);
            wildShot.AddLocalisation("Wild Shot");
            registry.RegisterCard(wildShot);

            var wildBarrage = new ExternalCard("rft.Dave.WildBarrageCard", typeof(WildBarrageCard), card_art_sprite, null);
            wildBarrage.AddLocalisation("Wild Barrage");
            registry.RegisterCard(wildBarrage);
        }

        public void LoadManifest(ICardOverwriteRegistry registry)
        {
            var new_meta = new CardMetaOverwrite("EWanderer.DemoMod.Meta")
            {
                Deck = ExternalDeck.GetRaw((int)Deck.dracula),
                DontLoc = false,
                DontOffer = false,
                ExtraGlossary = new string[] { "Help", "Why" },
                Rarity = (int)Rarity.rare,
                Unreleased = false,
                UpgradesTo = new int[] { (int)Upgrade.A, (int)Upgrade.B },
                WeirdCard = false
            };

            registry.RegisterCardMetaOverwrite(new_meta, typeof(CannonColorless).Name);

            var better_dodge = new PartialCardStatOverwrite("ewanderer.demomod.betterdodge", typeof(DodgeColorless)) { Cost = 0, Buoyant = true, Retain = true };

            registry.RegisterCardStatOverwrite(better_dodge);

            /*
            dbRegistry.RegisterCardMetaOverwrite(new_meta, typeof(CannonColorless).Name);
            var all_normal_cards = Assembly.GetAssembly(typeof(Card))?.GetTypes().Where(e => !e.IsAbstract && e.IsClass && e.IsSubclassOf(typeof(Card)));
            if (all_normal_cards != null)
            {
                foreach (var card_type in all_normal_cards)
                {
                    var zero_cost_overwrite = new PartialCardStatOverwrite("ewanderer.demomod.partialoverwrite." + card_type.Name, card_type);
                    zero_cost_overwrite.Cost = -1;
                    registry.RegisterCardStatOverwrite(zero_cost_overwrite);
                }
            }
            */
        }

        public void LoadManifest(ICharacterRegistry registry)
        {
            // var dracular_spr = ExternalSprite.GetRaw((int)Spr.panels_char_colorless);
            //
            // var start_cards = new Type[] { typeof(DraculaCard), typeof(DraculaCard) };
            // var playable_dracular_character = new ExternalCharacter("EWanderer.DemoMod.DracularChar", dracula_deck ?? throw new NullReferenceException(), dracular_spr, start_cards, new Type[0], default_animation ?? throw new NullReferenceException(), mini_animation ?? throw new NullReferenceException());
            // playable_dracular_character.AddNameLocalisation("Count Dracula");
            // playable_dracular_character.AddDescLocalisation("A vampire using blood magic to invoke the powers of the void.");
            // registry.RegisterCharacter(playable_dracular_character);
        }

        public void LoadManifest(IGlossaryRegisty registry)
        {
            var icon = ExternalSprite.GetRaw((int)Spr.icons_ace);
            var glossary = new ExternalGlossary("Ewanderer.DemoMod.DemoCard.Glossary", "ewandererdemocard", false, ExternalGlossary.GlossayType.action, icon);
            glossary.AddLocalisation("en", "EWDemoaction", "Have all the cheesecake in the world!");
            registry.RegisterGlossary(glossary);
            EWandererDemoAction.glossary_item = glossary.Head;

            var randomMoveFoeGlossary = new ExternalGlossary("rft.Dave.RandomMoveFoe.Glossary", "randommovefoe", false,
                ExternalGlossary.GlossayType.action, random_move_foe_sprite);
            randomMoveFoeGlossary.AddLocalisation("en", "random foe move", "Instantly move the opponent {0} spaces in a random direction.");
            registry.RegisterGlossary(randomMoveFoeGlossary);
            RandomMoveFoeAction.glossary_item = randomMoveFoeGlossary.Head;

            var blueOrangeGlossary = new ExternalGlossary("rft.Dave.BlueOrange.Glossary", "blueorange", false,
                ExternalGlossary.GlossayType.action, blue_orange_sprite);
            blueOrangeGlossary.AddLocalisation("en", "blue/orange", "When played, either performs the blue or orange actions.");
            registry.RegisterGlossary(blueOrangeGlossary);
            RandomChoiceActionFactory.glossary_item = blueOrangeGlossary.Head;
        }

        public void LoadManifest(IArtifactRegistry registry)
        {
            var spr = ExternalSprite.GetRaw((int)Spr.artifacts_AresCannon);
            var artifact = new ExternalArtifact(typeof(Artifacts.PortableBlackHole), "EWanderer.DemoMod.PortableBlackHoleArtifact", spr, null, new ExternalGlossary[0]);
            artifact.AddLocalisation("en", "Black Hole Generator 3000", "Bring your own black hole to a fight. Why would you bring it along? It will consume us all!");
            registry.RegisterArtifact(artifact);
        }

        public void LoadManifest(IStatusRegistry statusRegistry)
        {
            demo_status = new ExternalStatus("EWanderer.DemoMod.DoomStatus", false, System.Drawing.Color.Red, null, demo_status_sprite ?? throw new Exception("missing sprite"), false);
            statusRegistry.RegisterStatus(demo_status);
            demo_status.AddLocalisation("Radio", "We got a signal. Exciting!");
        }

        public void LoadManifest(ICustomEventHub eventHub)
        {
            // throw new NotImplementedException();
            eventHub.MakeEvent<Combat>("EWanderer.DemoMod.TestEvent");
            eventHub.ConnectToEvent<Combat>("EWanderer.DemoMod.TestEvent", (c) => { c.QueueImmediate(new ACardOffering() { amount = 10, battleType = BattleType.Elite, inCombat = true }); });
            ModManifest.EventHub = eventHub;
        }
    }
}