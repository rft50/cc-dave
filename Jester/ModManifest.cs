using CobaltCoreModding.Definitions;
using CobaltCoreModding.Definitions.ExternalItems;
using CobaltCoreModding.Definitions.ModContactPoints;
using CobaltCoreModding.Definitions.ModManifests;
using HarmonyLib;
using Jester.Actions;
using Jester.Cards;
using Jester.External;
using Microsoft.Extensions.Logging;

namespace Jester
{
    public class ModManifest : IModManifest, ISpriteManifest, IAnimationManifest, IDeckManifest, ICardManifest, ICardOverwriteManifest, ICharacterManifest, IGlossaryManifest, IArtifactManifest, IStatusManifest, ICustomEventManifest
    {
        public static ExternalStatus? demo_status;
        public static ICustomEventHub? EventHub;
        private ExternalSprite? card_art_sprite;
        private ExternalAnimation? default_animation;
        private ExternalSprite? demo_status_sprite;
        private ExternalSprite? DemoAttackSprite;
        public static ExternalDeck? JesterDeck;
        private ExternalSprite? dracular_art;
        private ExternalSprite? dracular_border;
        private ExternalAnimation? mini_animation;
        private ExternalSprite? mini_dracula_sprite;
        private ExternalSprite? pinker_per_border_over_sprite;

        public static IKokoroApi KokoroApi = null!;

        public IEnumerable<DependencyEntry> Dependencies => new DependencyEntry[]
        {
            new DependencyEntry<IModManifest>("Shockah.Kokoro")
        };
        public DirectoryInfo? GameRootFolder { get; set; }
        public ILogger? Logger { get; set; }
        public DirectoryInfo? ModRootFolder { get; set; }
        public string Name => "rft.Jester";

        public void BootMod(IModLoaderContact contact)
        {
            ReflectionExt.CurrentAssemblyLoadContext.LoadFromAssemblyPath(Path.Combine(ModRootFolder!.FullName, "Shrike.dll"));
            ReflectionExt.CurrentAssemblyLoadContext.LoadFromAssemblyPath(Path.Combine(ModRootFolder!.FullName, "Shrike.Harmony.dll"));

            KokoroApi = contact.GetApi<IKokoroApi>("Shockah.Kokoro")!;
            KokoroApi.RegisterTypeForExtensionData(typeof(Combat));
            
            var harmony = new Harmony("Jester");
            harmony.PatchAll();
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
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("Question.png"));
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
            default_animation = new ExternalAnimation("ewanderer.demomod.dracula.neutral", JesterDeck ?? throw new NullReferenceException(), "neutral", false, new ExternalSprite[] {
                ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_0),
                ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_1),
                ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_2),
                ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_3),
                ExternalSprite.GetRaw((int)Spr.characters_dracula_dracula_neutral_4),
            });

            registry.RegisterAnimation(default_animation);
            if (mini_dracula_sprite == null)
                throw new Exception();

            mini_animation = new ExternalAnimation("ewanderer.demomod.dracula.mini", JesterDeck, "mini", false, new ExternalSprite[] { mini_dracula_sprite });

            registry.RegisterAnimation(mini_animation);
        }

        public void LoadManifest(IDeckRegistry registry)
        {
            dracular_art = ExternalSprite.GetRaw((int)Spr.cards_colorless);
            dracular_border = ExternalSprite.GetRaw((int)Spr.cardShared_border_dracula);

            JesterDeck = new ExternalDeck("rft.Jester.JesterDeck", System.Drawing.Color.HotPink, System.Drawing.Color.Black, dracular_art ?? throw new NullReferenceException(), dracular_border ?? throw new NullReferenceException(), null);

            if (!registry.RegisterDeck(JesterDeck))
                return;
        }

        public void LoadManifest(ICardRegistry registry)
        {
            // ReSharper disable InconsistentNaming
            if (card_art_sprite == null)
                return;
            
            // common
            var j0o = new ExternalCard("rft.Jester.Joker0Offensive", typeof(Joker0Offensive), card_art_sprite, JesterDeck);
            j0o.AddLocalisation("Joker 0 Offensive");
            registry.RegisterCard(j0o);
            
            var j0d = new ExternalCard("rft.Jester.Joker0Defensive", typeof(Joker0Defensive), card_art_sprite, JesterDeck);
            j0d.AddLocalisation("Joker 0 Defensive");
            registry.RegisterCard(j0d);
            
            var j0u = new ExternalCard("rft.Jester.Joker0Utility", typeof(Joker0Utility), card_art_sprite, JesterDeck);
            j0u.AddLocalisation("Joker 0 Utility");
            registry.RegisterCard(j0u);
            
            var j1o = new ExternalCard("rft.Jester.Joker1Offensive", typeof(Joker1Offensive), card_art_sprite, JesterDeck);
            j1o.AddLocalisation("Joker 1 Offensive");
            registry.RegisterCard(j1o);
            
            var j1d = new ExternalCard("rft.Jester.Joker1Defensive", typeof(Joker1Defensive), card_art_sprite, JesterDeck);
            j1d.AddLocalisation("Joker 1 Defensive");
            registry.RegisterCard(j1d);
            
            var j1u = new ExternalCard("rft.Jester.Joker1Utility", typeof(Joker1Utility), card_art_sprite, JesterDeck);
            j1u.AddLocalisation("Joker 1 Utility");
            registry.RegisterCard(j1u);
            
            var j2o = new ExternalCard("rft.Jester.Joker2Offensive", typeof(Joker2Offensive), card_art_sprite, JesterDeck);
            j2o.AddLocalisation("Joker 2 Offensive");
            registry.RegisterCard(j2o);
            
            var j2d = new ExternalCard("rft.Jester.Joker2Defensive", typeof(Joker2Defensive), card_art_sprite, JesterDeck);
            j2d.AddLocalisation("Joker 2 Defensive");
            registry.RegisterCard(j2d);
            
            var j2u = new ExternalCard("rft.Jester.Joker2Utility", typeof(Joker2Utility), card_art_sprite, JesterDeck);
            j2u.AddLocalisation("Joker 2 Utility");
            registry.RegisterCard(j2u);
            
            // uncommon
            var j3o = new ExternalCard("rft.Jester.Joker3Offensive", typeof(Joker3Offensive), card_art_sprite, JesterDeck);
            j3o.AddLocalisation("Joker 3 Offensive");
            registry.RegisterCard(j3o);
            
            var j3d = new ExternalCard("rft.Jester.Joker3Defensive", typeof(Joker3Defensive), card_art_sprite, JesterDeck);
            j3d.AddLocalisation("Joker 3 Defensive");
            registry.RegisterCard(j3d);
            
            var j3u = new ExternalCard("rft.Jester.Joker3Utility", typeof(Joker3Utility), card_art_sprite, JesterDeck);
            j3u.AddLocalisation("Joker 3 Utility");
            registry.RegisterCard(j3u);

            var encore = new ExternalCard("rft.Jester.Encore", typeof(Encore), card_art_sprite, JesterDeck);
            encore.AddLocalisation("Encore");
            registry.RegisterCard(encore);

            var smokeAndMirrors = new ExternalCard("rft.Jester.SmokeAndMirrors", typeof(SmokeAndMirrors), card_art_sprite, JesterDeck);
            smokeAndMirrors.AddLocalisation("Smoke and Mirrors");
            registry.RegisterCard(smokeAndMirrors);

            var madCackle = new ExternalCard("rft.Jester.MadCackle", typeof(MadCackle), card_art_sprite, JesterDeck);
            madCackle.AddLocalisation("Mad Cackle");
            registry.RegisterCard(madCackle);
            
            // rare
            var j4o = new ExternalCard("rft.Jester.Joker4Offensive", typeof(Joker4Offensive), card_art_sprite, JesterDeck);
            j4o.AddLocalisation("Joker 4 Offensive");
            registry.RegisterCard(j4o);
            
            var j4d = new ExternalCard("rft.Jester.Joker4Defensive", typeof(Joker4Defensive), card_art_sprite, JesterDeck);
            j4d.AddLocalisation("Joker 4 Defensive");
            registry.RegisterCard(j4d);
            
            var j4u = new ExternalCard("rft.Jester.Joker4Utility", typeof(Joker4Utility), card_art_sprite, JesterDeck);
            j4u.AddLocalisation("Joker 4 Utility");
            registry.RegisterCard(j4u);
        }

        public void LoadManifest(ICardOverwriteRegistry registry)
        {
        }

        public void LoadManifest(ICharacterRegistry registry)
        {
            var dracular_spr = ExternalSprite.GetRaw((int)Spr.panels_char_colorless);
            
            var jester = new ExternalCharacter("rft.Jester.JesterChar", JesterDeck ?? throw new NullReferenceException(), dracular_spr, Type.EmptyTypes, Type.EmptyTypes, default_animation ?? throw new NullReferenceException(), mini_animation ?? throw new NullReferenceException());
            jester.AddNameLocalisation("Jester");
            jester.AddDescLocalisation("A mad jester. Only supposedly knows what he's doing.");
            registry.RegisterCharacter(jester);
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