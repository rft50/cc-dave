using CobaltCoreModding.Definitions;
using CobaltCoreModding.Definitions.ExternalItems;
using CobaltCoreModding.Definitions.ModContactPoints;
using CobaltCoreModding.Definitions.ModManifests;
using HarmonyLib;
using Jester.Api;
using Jester.Artifacts;
using Jester.Cards;
using Jester.External;
using Microsoft.Extensions.Logging;

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
    public static ExternalSprite CharacterFrame = null!;

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
    public static ILogger Logr = null!;

    public IEnumerable<DependencyEntry> Dependencies => new DependencyEntry[]
    {
        new DependencyEntry<IModManifest>("Shockah.Kokoro", false)
    };
    public DirectoryInfo? GameRootFolder { get; set; }
    public ILogger? Logger { get; set; }
    public DirectoryInfo? ModRootFolder { get; set; }
    public string Name => "rft.Jester";

    public static void Log(string @in)
    {
        Logr.Log(LogLevel.Information, new EventId(0), @in, null, (str, _) => str);
    }

    public void BootMod(IModLoaderContact contact)
    {
        ReflectionExt.CurrentAssemblyLoadContext.LoadFromAssemblyPath(Path.Combine(ModRootFolder!.FullName, "Shrike.dll"));
        ReflectionExt.CurrentAssemblyLoadContext.LoadFromAssemblyPath(Path.Combine(ModRootFolder!.FullName, "Shrike.Harmony.dll"));
        
        Logr = Logger!;

        KokoroApi = contact.GetApi<IKokoroApi>("Shockah.Kokoro")!;
        KokoroApi.RegisterTypeForExtensionData(typeof(Combat));

        JesterApi = new JesterApi();
        
        JesterApi.RegisterCardFlag("singleUse", request => request.CardData.singleUse);
        JesterApi.RegisterCardFlag("exhaust", request => request.Whitelist.Contains("mustExhaust") || request.CardData.exhaust || JesterApi.HasCardFlag("singleUse", request));
        
        JesterApi.RegisterCharacterFlag("heat", Deck.eunice);
        JesterApi.RegisterCharacterFlag("shard", Deck.shard);
            
        var harmony = new Harmony("rft.Jester");
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
            var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("char_frame_jester.png"));
            CharacterFrame = new ExternalSprite("rft.Jester.CharFrame", new FileInfo(path));
            if (!artRegistry.RegisterArt(CharacterFrame))
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
        var jco = new ExternalCard("rft.Jester.CommonOffensiveJoker", typeof(CommonOffensiveJoker), card_art_sprite, JesterDeck);
        jco.AddLocalisation("Common Offensive Joker");
        registry.RegisterCard(jco);
            
        var jcd = new ExternalCard("rft.Jester.CommonDefensiveJoker", typeof(CommonDefensiveJoker), card_art_sprite, JesterDeck);
        jcd.AddLocalisation("Common Defensive Joker");
        registry.RegisterCard(jcd);
            
        var jcu = new ExternalCard("rft.Jester.CommonUtilityJoker", typeof(CommonUtilityJoker), card_art_sprite, JesterDeck);
        jcu.AddLocalisation("Common Utility Joker");
        registry.RegisterCard(jcu);
            
        // uncommon
        var juo = new ExternalCard("rft.Jester.UncommonOffensiveJoker", typeof(UncommonOffensiveJoker), card_art_sprite, JesterDeck);
        juo.AddLocalisation("Uncommon Offensive Joker");
        registry.RegisterCard(juo);
            
        var jud = new ExternalCard("rft.Jester.UncommonDefensiveJoker", typeof(UncommonDefensiveJoker), card_art_sprite, JesterDeck);
        jud.AddLocalisation("Uncommon Defensive Joker");
        registry.RegisterCard(jud);
            
        var juu = new ExternalCard("rft.Jester.UncommonUtilityJoker", typeof(UncommonUtilityJoker), card_art_sprite, JesterDeck);
        juu.AddLocalisation("Uncommon Utility Joker");
        registry.RegisterCard(juu);

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
        var jro = new ExternalCard("rft.Jester.RareOffensiveJoker", typeof(RareOffensiveJoker), card_art_sprite, JesterDeck);
        jro.AddLocalisation("Rare Joker Offensive");
        registry.RegisterCard(jro);
            
        var jrd = new ExternalCard("rft.Jester.RareDefensiveJoker", typeof(RareDefensiveJoker), card_art_sprite, JesterDeck);
        jrd.AddLocalisation("Rare Joker Defensive");
        registry.RegisterCard(jrd);
            
        var jru = new ExternalCard("rft.Jester.RareUtilityJoker", typeof(RareUtilityJoker), card_art_sprite, JesterDeck);
        jru.AddLocalisation("Rare Joker Utility");
        registry.RegisterCard(jru);
            
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
        var jester_panel = CharacterFrame;
        
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