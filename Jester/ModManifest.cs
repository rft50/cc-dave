using System.Reflection.Emit;
using CobaltCoreModding.Definitions;
using CobaltCoreModding.Definitions.ExternalItems;
using CobaltCoreModding.Definitions.ModContactPoints;
using CobaltCoreModding.Definitions.ModManifests;
using HarmonyLib;
using Jester.Actions;
using Jester.Api;
using Jester.Artifacts;
using Jester.Cards;
using Jester.External;
using Microsoft.Extensions.Logging;
using Nanoray.Shrike.Harmony;

namespace Jester;

public class ModManifest : IModManifest, ISpriteManifest, IAnimationManifest, IDeckManifest, ICardManifest, ICardOverwriteManifest, ICharacterManifest, IGlossaryManifest, IArtifactManifest, IStatusManifest, ICustomEventManifest, IApiProviderManifest
{
    public static ICustomEventHub? EventHub;
    private ExternalSprite? card_art_sprite;
    private ExternalAnimation? default_animation;
    private ExternalSprite? demo_status_sprite;
    public static ExternalDeck? JesterDeck;
    public static ExternalSprite CardFrame = null!;
    public static ExternalAnimation MiniAnimation = null!;
    public static ExternalSprite JesterMini = null!;

    public static ExternalSprite BellSprite = null!;

    public static Dictionary<string, ExternalSprite> ArtifactSprites = new();
    public static Dictionary<string, ExternalSprite> CharacterSprites = new();

    public static ExternalArtifact OpeningScript = null!;
    public static ExternalArtifact ClosingCeremony = null!;

    public static ExternalStatus OpeningFatigue = null!;

    public static ExternalGlossary OpeningScriptedGlossary = null!;
    public static ExternalGlossary JesterRandomGlossary = null!;

    public static Tooltip OpeningScriptedTooltip = null!;
    public static Tooltip JesterRandomTooltip = null!;

    public static IKokoroApi KokoroApi = null!;
    public static IJesterApi JesterApi = null!;

    public IEnumerable<DependencyEntry> Dependencies => new DependencyEntry[]
    {
        new DependencyEntry<IModManifest>("Shockah.Kokoro", false)
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

        JesterApi = new JesterApi();
        
        JesterApi.RegisterCardFlag("singleUse", request => request.CardData.singleUse);
        JesterApi.RegisterCardFlag("exhaust", request => request.Whitelist.Contains("mustExhaust") || request.CardData.exhaust || JesterApi.HasCardFlag("singleUse", request));
        
        JesterApi.RegisterCharacterFlag("heat", Deck.eunice);
        JesterApi.RegisterCharacterFlag("shard", Deck.shard);
            
        var harmony = new Harmony("Jester");
        harmony.PatchAll();
    }

    public void LoadManifest(ISpriteRegistry artRegistry)
    {
        if (ModRootFolder == null)
            throw new Exception("No root folder set!");

        {
            var path = Path.Combine(ModRootFolder.FullName, "Sprites", "Card", Path.GetFileName("Question.png"));
            card_art_sprite = new ExternalSprite("rft.Jester.Question", new FileInfo(path));
            if (!artRegistry.RegisterArt(card_art_sprite))
                throw new Exception("Cannot register sprite.");
        }
        {
            var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("sorwest_frame.png"));
            CardFrame = new ExternalSprite("rft.Jester.Frame", new FileInfo(path));
            if (!artRegistry.RegisterArt(CardFrame))
                throw new Exception("Cannot register sprite.");
        }
        {
            var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("jester_mini_insane.png"));
            JesterMini = new ExternalSprite("rft.Jester.mini", new FileInfo(path));
            if (!artRegistry.RegisterArt(JesterMini))
                throw new Exception("Cannot register sprite.");
        }
        {
            var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("bell.png"));
            BellSprite = new ExternalSprite("rft.Jester.Bell", new FileInfo(path));
            if (!artRegistry.RegisterArt(BellSprite))
                throw new Exception("Cannot register sprite.");
        }

        {
            var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("demo_status.png"));
            demo_status_sprite = new ExternalSprite("EWanderer.DemoMod.demo_status.sprite", new FileInfo(path));
            if (!artRegistry.RegisterArt(demo_status_sprite))
                throw new Exception("Cannot register sprite.");
        }

        {
            var artifactList = new List<string>
            {
                "script",
                "ceremony"
            };
                
            foreach (var artifact in artifactList)
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", "Artifact", Path.GetFileName($"{artifact}.png"));
                var sprite = new ExternalSprite($"rft.Jester.Artifact.{artifact}", new FileInfo(path));
                if (!artRegistry.RegisterArt(sprite))
                    throw new Exception($"Cannot register Artifact sprite {artifact}");
                ArtifactSprites[artifact] = sprite;
            }
        }

        {
            var frames = new List<int> { 1, 2, 3, 4 };
            
            var characterList = new List<string>
            {
                "calm_neutral",
                "calm_squint",
                "hat",
                "insane_neutral",
                "insane_squint"
            }.SelectMany(s => frames.Select(i => s + i))
            .AddItem("HAT");
            
                
            foreach (var character in characterList)
            {
                var path = Path.Combine(ModRootFolder.FullName, "Sprites", "Character", Path.GetFileName($"jester_squirre_{character}.png"));
                var sprite = new ExternalSprite($"rft.Jester.Character.{character}", new FileInfo(path));
                if (!artRegistry.RegisterArt(sprite))
                    throw new Exception($"Cannot register Character sprite {character}");
                CharacterSprites[character] = sprite;
            }
        }
    }

    public void LoadManifest(IAnimationRegistry registry)
    {
        default_animation = new ExternalAnimation("rft.Jester.Anim.neutral", JesterDeck ?? throw new NullReferenceException(), "neutral", false, new ExternalSprite[] {
            CharacterSprites["insane_neutral1"],
            CharacterSprites["insane_neutral2"],
            CharacterSprites["insane_neutral3"],
            CharacterSprites["insane_neutral4"]
        });

        registry.RegisterAnimation(default_animation);

        MiniAnimation = new ExternalAnimation("rft.Jester.Anim.mini", JesterDeck, "mini", false, new ExternalSprite[] { JesterMini });

        registry.RegisterAnimation(MiniAnimation);
    }

    public void LoadManifest(IDeckRegistry registry)
    {
        JesterDeck = new ExternalDeck("rft.Jester.JesterDeck", System.Drawing.Color.FromArgb(159, 0, 40), System.Drawing.Color.Black, card_art_sprite ?? throw new NullReferenceException(), CardFrame ?? throw new NullReferenceException(), null);

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

        var curtainCall = new ExternalCard("rft.Jester.CurtainCall", typeof(CurtainCall), card_art_sprite, JesterDeck);
        curtainCall.AddLocalisation("Curtain Call");
        registry.RegisterCard(curtainCall);

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
            
        var openingAct = new ExternalCard("rft.Jester.OpeningAct", typeof(OpeningAct), card_art_sprite, JesterDeck);
        openingAct.AddLocalisation("Opening Act");
        registry.RegisterCard(openingAct);
            
        var finalAct = new ExternalCard("rft.Jester.FinalAct", typeof(FinalAct), card_art_sprite, JesterDeck);
        finalAct.AddLocalisation("Final Act");
        registry.RegisterCard(finalAct);
    }

    public void LoadManifest(ICardOverwriteRegistry registry)
    {
    }

    public void LoadManifest(ICharacterRegistry registry)
    {
        var jester_panel = ExternalSprite.GetRaw((int)Spr.panels_char_colorless);
            
        var jester = new ExternalCharacter("rft.Jester.JesterChar", JesterDeck ?? throw new NullReferenceException(), jester_panel, Type.EmptyTypes, Type.EmptyTypes, default_animation ?? throw new NullReferenceException(), MiniAnimation ?? throw new NullReferenceException());
        jester.AddNameLocalisation("Jester");
        jester.AddDescLocalisation("A mad jester. Only supposedly knows what he's doing.");
        registry.RegisterCharacter(jester);
    }

    public void LoadManifest(IGlossaryRegisty registry)
    {
        var icon = ExternalSprite.GetRaw((int)Spr.icons_ace);

        OpeningScriptedGlossary = new ExternalGlossary("rft.Jester.OpeningScripted.Glossary", "OpeningScripted",
            false, ExternalGlossary.GlossayType.cardtrait, icon);
        OpeningScriptedGlossary.AddLocalisation("en", "Opening Scripted", "Plays itself at the start of each combat, as long as it remains in your deck. Gain Opening Fatigue equal to its cost.");
        registry.RegisterGlossary(OpeningScriptedGlossary);
        OpeningScriptedTooltip = new TTGlossary(OpeningScriptedGlossary.Head);

        JesterRandomGlossary = new ExternalGlossary("rft.Jester.JesterRandom.Glossary", "JesterRandom", false,
            ExternalGlossary.GlossayType.cardtrait, BellSprite);
        JesterRandomGlossary.AddLocalisation("en", "Jester Random",
            "The Jester's interpretation of this type of card. It's always different.");
        registry.RegisterGlossary(JesterRandomGlossary);
        JesterRandomTooltip = new TTGlossary(JesterRandomGlossary.Head);
    }

    public void LoadManifest(IArtifactRegistry registry)
    {
        // FROM CARDS
        {
            OpeningScript = new ExternalArtifact("rft.Jester.OpeningScript", typeof(OpeningScript),
                ArtifactSprites["script"], ownerDeck: JesterDeck);
            OpeningScript.AddLocalisation("OPENING SCRIPT", "At the start of combat, play every card on the opening script in order, then gain Opening Fatigue equal to their costs, minus their discounts.");
            registry.RegisterArtifact(OpeningScript);

            ClosingCeremony = new ExternalArtifact("rft.Jester.ClosingCeremony", typeof(ClosingCeremony),
                ArtifactSprites["ceremony"], ownerDeck: JesterDeck);
            ClosingCeremony.AddLocalisation("CLOSING CEREMONY", "At the start of the next boss, add Final Act Bs to your deck. The number states how many.");
            registry.RegisterArtifact(ClosingCeremony);
        }
    }

    public void LoadManifest(IStatusRegistry statusRegistry)
    {
        OpeningFatigue = new ExternalStatus("rft.Jester.OpeningFatigue", false, System.Drawing.Color.Red, null,
            demo_status_sprite!, true);
        statusRegistry.RegisterStatus(OpeningFatigue);
        OpeningFatigue.AddLocalisation("Opening Fatigue", "At the start of the next {0} turns, lose 1 energy.");
    }

    public void LoadManifest(ICustomEventHub eventHub)
    {
    }

    public object GetApi(IManifest requester) => JesterApi;
}