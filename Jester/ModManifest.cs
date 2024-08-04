using CobaltCoreModding.Definitions;
using CobaltCoreModding.Definitions.ExternalItems;
using CobaltCoreModding.Definitions.ModContactPoints;
using CobaltCoreModding.Definitions.ModManifests;
using HarmonyLib;
using Jester.Api;
using Jester.Artifacts;
using Jester.Cards;
using Jester.External;
using Jester.Generator;
using Jester.Render;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using Nickel.Legacy;
using IModManifest = CobaltCoreModding.Definitions.ModManifests.IModManifest;
using ReflectionExt = Jester.External.ReflectionExt;

namespace Jester;

public class ModManifest : IModManifest, ISpriteManifest, IAnimationManifest, IDeckManifest, ICardManifest, ICardOverwriteManifest, ICharacterManifest, IGlossaryManifest, IArtifactManifest, IStatusManifest, ICustomEventManifest, IApiProviderManifest, INickelManifest
{
    public static ICustomEventHub EventHub = null!;
    public static IModHelper Helper = null!;
    internal static Settings Settings = null!;
    private ExternalSprite? card_art_sprite;
    private ExternalAnimation? default_animation;
    private ExternalSprite? demo_status_sprite;
    public static ExternalDeck? JesterDeck;
    public static ExternalSprite CardFrame = null!;
    public static ExternalAnimation MiniAnimation = null!;
    public static ExternalSprite JesterMini = null!;

    public static ExternalSprite BellSprite = null!;
    public static ExternalSprite ScriptedSprite = null!;
    public static ExternalSprite CharacterFrame = null!;

    public static Dictionary<string, ExternalSprite> ArtifactSprites = new();
    public static Dictionary<string, ExternalSprite> CharacterSprites = new();

    public static ExternalArtifact OpeningScript = null!;
    public static ExternalArtifact ClosingCeremony = null!;
    public static ExternalArtifact CherishedPhoto = null!;
    public static ExternalArtifact JingleBells = null!;
    public static ExternalArtifact JugglingBalls = null!;
    public static ExternalArtifact Confetti = null!;

    public static ExternalStatus OpeningFatigue = null!;

    public static ExternalGlossary OpeningScriptedGlossary = null!;
    public static ExternalGlossary JesterRandomGlossary = null!;

    public static Tooltip OpeningScriptedTooltip = null!;
    public static Tooltip JesterRandomTooltip = null!;

    public static IKokoroApi KokoroApi = null!;
    public static IJesterApi JesterApi = null!;
    public static IMoreDifficultiesApi? MoreDifficultiesApi;
    public static ILogger Logr = null!;

    public IEnumerable<DependencyEntry> Dependencies => new DependencyEntry[]
    {
        new DependencyEntry<IModManifest>("Shockah.Kokoro", false),
        new DependencyEntry<IModManifest>("TheJazMaster.MoreDifficulties", true)
    };
    public DirectoryInfo? GameRootFolder { get; set; }
    public ILogger? Logger { get; set; }
    public DirectoryInfo? ModRootFolder { get; set; }
    public string Name => "rft.Jester";
    public void OnNickelLoad(IPluginPackage<Nickel.IModManifest> package, IModHelper helper)
    {
        Helper = helper;
        Settings = new Settings();
        
        helper.Events.RegisterBeforeArtifactsHook(nameof(Artifact.OnCombatEnd), (State state) =>
        {
            if (!state.EnumerateAllArtifacts().Any(a => a is JugglingBalls)) return;
            foreach (var card in state.deck)
            {
                if (card is not AbstractJoker joker) continue;
                joker.Seed = null;
                joker.Energy = null;
                joker.ClearCache();
            }
        }, 0);
        
        helper.ModRegistry.AwaitApi<IModSettingsApi>("Nickel.ModSettings", ms =>
        {
            var options = new List<IModSettingsApi.IModSetting>();
            
            options.Add(ms.MakeHeader(() => "These will only apply at the start of new runs"));
                
            options.Add(ms.MakeCheckbox(() => "Insane Mode", () => Settings.ProfileBased.Current.InsaneMode, (g, ctx, val) =>
            {
                Settings.ProfileBased.Current.InsaneMode = val;
            }));
            
            options.Add(ms.MakeNumericStepper(() => "Max Actions", () => Settings.ProfileBased.Current.ActionCap, val =>
            {
                Settings.ProfileBased.Current.ActionCap = val;
            }, 5, 7));

            var conditionalOptions = new List<IModSettingsApi.IModSetting>();
            
            conditionalOptions.Add(ms.MakeHeader(() => "These are debug options for Jester integration. They print directly to the logs."));
            
            conditionalOptions.Add(ms.MakeButton(() => "Character Flags", (_, _) =>
            {
                var flags = ((JesterApi)JesterApi).CharacterFlags;
                Logr.LogDebug("Character Flags:");
                foreach (var (key, value) in flags)
                {
                    var chars = value.Select(Helper.Content.Decks.LookupByDeck)
                        .Where(c => c != null)
                        .Select(c => c!.UniqueName);
                    Logr.LogDebug(key + ": " + string.Join(", ", chars));
                }
            }));
            
            conditionalOptions.Add(ms.MakeButton(() => "Entry Tags", (_, _) =>
            {
                ((JesterApi)JesterApi).ForceAllCharFlags = true;
                
                var basicRequest = new JesterRequest
                {
                    Seed = 0,
                    FirstAction = null,
                    State = DB.fakeState,
                    BasePoints = 150,
                    CardData = new CardData
                    {
                        exhaust = false,
                        singleUse = false
                    },
                    ActionLimit = 5,
                    SingleUse = false,
                    CardMeta = new CardMeta
                    {
                        rarity = Rarity.common
                    }
                };
                var exhaustRequest = new JesterRequest
                {
                    Seed = 0,
                    FirstAction = null,
                    State = DB.fakeState,
                    BasePoints = 150,
                    CardData = new CardData
                    {
                        exhaust = true,
                        singleUse = false
                    },
                    ActionLimit = 5,
                    SingleUse = false,
                    CardMeta = new CardMeta
                    {
                        rarity = Rarity.uncommon
                    }
                };
                var singleUseRequest = new JesterRequest
                {
                    Seed = 0,
                    FirstAction = null,
                    State = DB.fakeState,
                    BasePoints = 150,
                    CardData = new CardData
                    {
                        exhaust = false,
                        singleUse = true
                    },
                    ActionLimit = 5,
                    SingleUse = true,
                    CardMeta = new CardMeta
                    {
                        rarity = Rarity.rare
                    }
                };
                var costRequest = new JesterRequest
                {
                    Seed = 0,
                    FirstAction = null,
                    State = DB.fakeState,
                    BasePoints = 150,
                    CardData = new CardData
                    {
                        exhaust = false,
                        singleUse = false
                    },
                    ActionLimit = 5,
                    SingleUse = false,
                    CardMeta = new CardMeta
                    {
                        rarity = Rarity.rare
                    },
                    Whitelist = new HashSet<string>
                    {
                        "cost"
                    }
                };

                List<IJesterApi.IJesterRequest> requests = [basicRequest, exhaustRequest, singleUseRequest, costRequest];
                var providers = JesterGenerator.Providers;
                var weightedOptions = providers.SelectMany(p => requests.SelectMany(p.GetEntries))
                    .SelectMany<(double, IJesterApi.IEntry), (string, double)>(we =>
                    {
                        var w = we.Item1;
                        var e = we.Item2;
                        return e.Tags.Select(t => (t, w));
                    });
                var results = new Dictionary<string, double>();
                foreach (var (tag, weight) in weightedOptions)
                {
                    results.TryAdd(tag, 0);
                    results[tag] += weight;
                }
                Logr.LogDebug("Entry Tags:");
                foreach (var (key, value) in results.OrderBy(e => e.Value).Reverse())
                {
                    Logr.LogDebug(key + ": " + value);
                }
                
                ((JesterApi)JesterApi).ForceAllCharFlags = false;
            }));
            
            options.Add(ms.MakeConditional(
                ms.MakeList(conditionalOptions),
                () => FeatureFlags.Debug
                ));
            
            ms.RegisterModSettings(
                ms.MakeList(options)
            );
        });
    }

    public void BootMod(IModLoaderContact contact)
    {
        ReflectionExt.CurrentAssemblyLoadContext.LoadFromAssemblyPath(Path.Combine(ModRootFolder!.FullName, "Shrike.dll"));
        ReflectionExt.CurrentAssemblyLoadContext.LoadFromAssemblyPath(Path.Combine(ModRootFolder!.FullName, "Shrike.Harmony.dll"));
        
        Logr = Logger!;

        KokoroApi = contact.GetApi<IKokoroApi>("Shockah.Kokoro")!;
        KokoroApi.RegisterCardRenderHook(new ZipperCardRenderManager(), 0);
        
        MoreDifficultiesApi = contact.GetApi<IMoreDifficultiesApi>("TheJazMaster.MoreDifficulties");

        JesterApi = new JesterApi();
        
        JesterApi.RegisterCardFlag("singleUse", request => request.CardData.singleUse);
        JesterApi.RegisterCardFlag("exhaust", request => request.Whitelist.Contains("mustExhaust") || request.CardData.exhaust || JesterApi.HasCardFlag("singleUse", request));
        
        JesterApi.RegisterCharacterFlag("midrow", Deck.goat);
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
            var path = Path.Combine(ModRootFolder.FullName, "Sprites", Path.GetFileName("scripted.png"));
            ScriptedSprite = new ExternalSprite("rft.Jester.Scripted", new FileInfo(path));
            if (!artRegistry.RegisterArt(ScriptedSprite))
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
                "ceremony",
                "cherishedphoto",
                "confetti",
                "jinglebells",
                "jugglingballs",
                "jugglingballs_off",
                "script"
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

        MoreDifficultiesApi?.RegisterAltStarters((Deck)JesterDeck.Id!, new StarterDeck());
    }

    public void LoadManifest(ICardRegistry registry)
    {
        // ReSharper disable InconsistentNaming
        if (card_art_sprite == null)
            return;
            
        // common
        var jco = new ExternalCard("rft.Jester.CommonOffensiveJoker", typeof(CommonOffensiveJoker), card_art_sprite, JesterDeck);
        jco.AddLocalisation("Common Offensive");
        registry.RegisterCard(jco);
            
        var jcd = new ExternalCard("rft.Jester.CommonDefensiveJoker", typeof(CommonDefensiveJoker), card_art_sprite, JesterDeck);
        jcd.AddLocalisation("Common Defensive");
        registry.RegisterCard(jcd);
            
        var jcu = new ExternalCard("rft.Jester.CommonUtilityJoker", typeof(CommonUtilityJoker), card_art_sprite, JesterDeck);
        jcu.AddLocalisation("Common Utility");
        registry.RegisterCard(jcu);
            
        // uncommon
        var juo = new ExternalCard("rft.Jester.UncommonOffensiveJoker", typeof(UncommonOffensiveJoker), card_art_sprite, JesterDeck);
        juo.AddLocalisation("Uncommon Offensive");
        registry.RegisterCard(juo);
            
        var jud = new ExternalCard("rft.Jester.UncommonDefensiveJoker", typeof(UncommonDefensiveJoker), card_art_sprite, JesterDeck);
        jud.AddLocalisation("Uncommon Defensive");
        registry.RegisterCard(jud);
            
        var juu = new ExternalCard("rft.Jester.UncommonUtilityJoker", typeof(UncommonUtilityJoker), card_art_sprite, JesterDeck);
        juu.AddLocalisation("Uncommon Utility");
        registry.RegisterCard(juu);

        var encore = new ExternalCard("rft.Jester.Encore", typeof(Encore), card_art_sprite, JesterDeck);
        encore.AddLocalisation("Encore");
        registry.RegisterCard(encore);

        var smokeAndMirrors = new ExternalCard("rft.Jester.SmokeAndMirrors", typeof(SmokeAndMirrors), card_art_sprite, JesterDeck);
        smokeAndMirrors.AddLocalisation("Smoke and Mirrors");
        registry.RegisterCard(smokeAndMirrors);
        
        var practice = new ExternalCard("rft.Jester.Practice", typeof(Practice), card_art_sprite, JesterDeck);
        practice.AddLocalisation("Practice");
        registry.RegisterCard(practice);

        var madCackle = new ExternalCard("rft.Jester.MadCackle", typeof(MadCackle), card_art_sprite, JesterDeck);
        madCackle.AddLocalisation("Mad Cackle");
        registry.RegisterCard(madCackle);
            
        // rare
        var jro = new ExternalCard("rft.Jester.RareOffensiveJoker", typeof(RareOffensiveJoker), card_art_sprite, JesterDeck);
        jro.AddLocalisation("Rare Offensive");
        registry.RegisterCard(jro);
            
        var jrd = new ExternalCard("rft.Jester.RareDefensiveJoker", typeof(RareDefensiveJoker), card_art_sprite, JesterDeck);
        jrd.AddLocalisation("Rare Defensive");
        registry.RegisterCard(jrd);
            
        var jru = new ExternalCard("rft.Jester.RareUtilityJoker", typeof(RareUtilityJoker), card_art_sprite, JesterDeck);
        jru.AddLocalisation("Rare Utility");
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
        jester.AddDescLocalisation("<c=9f0028>JESTER</c>\nA mad jester. He only supposedly knows what he's doing.");
        registry.RegisterCharacter(jester);
    }

    public void LoadManifest(IGlossaryRegisty registry)
    {
        var icon = ScriptedSprite;

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
            OpeningScript.AddLocalisation("OPENING SCRIPT", "At the start of combat, play every card on the opening script in order, then gain <c=status>Opening Fatigue</c> equal to their costs, minus their discounts.");
            registry.RegisterArtifact(OpeningScript);

            ClosingCeremony = new ExternalArtifact("rft.Jester.ClosingCeremony", typeof(ClosingCeremony),
                ArtifactSprites["ceremony"], ownerDeck: JesterDeck);
            ClosingCeremony.AddLocalisation("CLOSING CEREMONY", "At the start of the next boss, add Final Act Bs to your deck. The number states how many.");
            registry.RegisterArtifact(ClosingCeremony);
        }
        // COMMON ARTIFACTS
        {
            CherishedPhoto = new ExternalArtifact("rft.Jester.CherishedPhoto", typeof(CherishedPhoto),
                ArtifactSprites["cherishedphoto"], ownerDeck: JesterDeck);
            CherishedPhoto.AddLocalisation("CHERISHED PHOTO", "At the end of combat, pick a <c=9f0028>Jester</c> card in your deck. Keep it, or pick one of four similar cards to replace it.");
            registry.RegisterArtifact(CherishedPhoto);
            
            JingleBells = new ExternalArtifact("rft.Jester.JingleBells", typeof(JingleBells),
                ArtifactSprites["jinglebells"], ownerDeck: JesterDeck);
            JingleBells.AddLocalisation("JINGLE BELLS", "Whenever you play any given card for the second time on a turn, gain 1 <c=energy>ENERGY</c>.");
            registry.RegisterArtifact(JingleBells);
        }
        // BOSS ARTIFACTS
        {
            JugglingBalls = new ExternalArtifact("rft.Jester.JugglingBalls", typeof(JugglingBalls),
                ArtifactSprites["jugglingballs"], ownerDeck: JesterDeck);
            JugglingBalls.AddLocalisation("JUGGLING BALLS", "The first time you play a <c=9f0028>Jester</c> card each turn, gain 1 <c=energy>ENERGY</c>.\n<c=downside>At the end of every combat, reroll every Jester card in your deck.</c>");
            registry.RegisterArtifact(JugglingBalls);
            Artifacts.JugglingBalls.ReadySpr = (Spr) ArtifactSprites["jugglingballs"].Id!;
            Artifacts.JugglingBalls.NotReadySpr = (Spr) ArtifactSprites["jugglingballs_off"].Id!;
            
            Confetti = new ExternalArtifact("rft.Jester.Confetti", typeof(Confetti),
                ArtifactSprites["confetti"], ownerDeck: JesterDeck);
            Confetti.AddLocalisation("CONFETTI", "Every three cards you exhaust, add a <c=cardtrait>Single Use</c> Encore to your hand.");
            registry.RegisterArtifact(Confetti);
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