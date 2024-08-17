using Dave.Actions;
using Dave.Api;
using Dave.Artifacts;
using Dave.Artifacts.Duos;
using Dave.Cards;
using Dave.External;
using Dave.Jester;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;

namespace Dave;

public class ModEntry : SimpleMod
{
    internal static ModEntry Instance { get; private set; } = null!;
    internal readonly Harmony Harmony;
    // dave API
    internal readonly IKokoroApi KokoroApi;
    internal readonly IMoreDifficultiesApi? MoreDifficultiesApi;
    internal readonly IJesterApi? JesterApi;
    internal readonly IDraculaApi? DraculaApi;
    internal readonly ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations;
    internal readonly ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations;

    internal readonly IStatusEntry RedRigging;
    internal readonly IStatusEntry BlackRigging;
    internal readonly IStatusEntry RedBias;
    internal readonly IStatusEntry BlackBias;
    internal readonly HookManager<IDaveApi.IRollHook> RollHookManager;
    internal readonly HookManager<IDaveApi.IRollModifier> RollModifierManager;

    internal static readonly IReadOnlyList<Type> CommonCards =
    [
        typeof(LuckyEscapeCard),
        typeof(RiggingShotCard),
        typeof(WildStepCard),
        typeof(WildShotCard),
        typeof(WindupCard),
        typeof(PrimedShotCard),
        typeof(PinchShotCard),
        typeof(WildWallCard),
        typeof(FoldCard)
    ];

    internal static readonly IReadOnlyList<Type> UncommonCards =
    [
        typeof(WildBarrageCard),
        typeof(RiggingCard),
        typeof(RaiseCard),
        typeof(InvestmentCard),
        typeof(LowballCard),
        typeof(LuckyShotCard),
        typeof(SeeingRedCard)
    ];

    internal static readonly IReadOnlyList<Type> RareCards =
    [
        typeof(LoadedDiceCard),
        typeof(AllInCard),
        typeof(EvenBetCard),
        typeof(AllBetsAreOffCard),
        typeof(DrawnGambitCard)
    ];

    internal static readonly IReadOnlyList<Type> DuoArtifacts =
    [
        typeof(DaveDizzyDuoArtifact),
        typeof(DaveRiggsDuoArtifact),
        typeof(DavePeriDuoArtifact),
        typeof(DaveIsaacDuoArtifact),
        typeof(DaveDrakeDuoArtifact),
        typeof(DaveMaxDuoArtifact),
        typeof(DaveBooksDuoArtifact),
        typeof(DaveCatDuoArtifact),
        
        typeof(DaveDynaDuoArtifact),
        typeof(DaveDraculaDuoArtifact),
        typeof(DaveEddieDuoArtifact),
        typeof(DaveJohnsonDuoArtifact),
        typeof(DaveSogginsDuoArtifact)
    ];
    
    internal IDeckEntry DaveDeck { get; }
    
    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
    {
        Instance = this;
        Harmony = new Harmony(package.Manifest.UniqueName);
        KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!;
        MoreDifficultiesApi = helper.ModRegistry.GetApi<IMoreDifficultiesApi>("TheJazMaster.MoreDifficulties");
        JesterApi = helper.ModRegistry.GetApi<IJesterApi>("rft.Jester");
        DraculaApi = helper.ModRegistry.GetApi<IDraculaApi>("Shockah.Dracula");
        RollHookManager = new HookManager<IDaveApi.IRollHook>();
        RollModifierManager = new HookManager<IDaveApi.IRollModifier>();
        
        RollModifierManager.Register(new RiggingRollModifier(), 0);

        AnyLocalizations = new JsonLocalizationProvider(
            tokenExtractor: new SimpleLocalizationTokenExtractor(),
            localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"i18n/{locale}.json").OpenRead()
        );
        Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
            new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(this.AnyLocalizations)
        );
        
        DaveDeck = helper.Content.Decks.RegisterDeck("Dave", new DeckConfiguration
        {
            Definition = new DeckDef { color = new Color("98FB98"), titleColor = Colors.black },
            DefaultCardArt = StableSpr.cards_colorless,
            BorderSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/frame_dave.png")).Sprite,
            Name = AnyLocalizations.Bind(["character", "name"]).Localize
        });
        
        // card registration
        // common
        RegisterCard(package, helper, typeof(LuckyEscapeCard), "LuckyEscape");
        RegisterCard(package, helper, typeof(RiggingShotCard), "RiggingShot");
        RegisterCard(package, helper, typeof(WildStepCard), "WildStep");
        RegisterCard(package, helper, typeof(WildShotCard), "WildShot");
        RegisterCard(package, helper, typeof(WindupCard), "Windup");
        RegisterCard(package, helper, typeof(PrimedShotCard), "PrimedShot");
        RegisterCard(package, helper, typeof(PinchShotCard), "PinchShot");
        RegisterCard(package, helper, typeof(WildWallCard), "WildWall");
        RegisterCard(package, helper, typeof(FoldCard), "Fold");
        // uncommon
        RegisterCard(package, helper, typeof(WildBarrageCard), "WildBarrage");
        RegisterCard(package, helper, typeof(RiggingCard), "Rigging");
        RiggingCard.red_sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Card/Rigging_Red.png")).Sprite;
        RiggingCard.black_sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Card/Rigging_Black.png")).Sprite;
        RegisterCard(package, helper, typeof(RaiseCard), "Raise");
        RegisterCard(package, helper, typeof(InvestmentCard), "Investment");
        RegisterCard(package, helper, typeof(LowballCard), "Lowball");
        RegisterCard(package, helper, typeof(LuckyShotCard), "LuckyShot");
        RegisterCard(package, helper, typeof(SeeingRedCard), "SeeingRed");
        // rare
        RegisterCard(package, helper, typeof(LoadedDiceCard), "LoadedDice");
        LoadedDiceCard.red_sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Card/LoadedDice_Red.png")).Sprite;
        LoadedDiceCard.black_sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Card/LoadedDice_Black.png")).Sprite;
        RegisterCard(package, helper, typeof(AllInCard), "AllIn");
        RegisterCard(package, helper, typeof(EvenBetCard), "EvenBet");
        RegisterCard(package, helper, typeof(AllBetsAreOffCard), "AllBetsAreOff");
        RegisterCard(package, helper, typeof(DrawnGambitCard), "DrawnGambit");
        // special
        RegisterCard(package, helper, typeof(PerfectOddsCard), "PerfectOdds", false);
        PerfectOddsCard.red_sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Card/PerfectOdds_Red.png")).Sprite;
        PerfectOddsCard.black_sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Card/PerfectOdds_Black.png")).Sprite;
        
        // character registration
        helper.Content.Characters.V2.RegisterPlayableCharacter("Dave", new PlayableCharacterConfigurationV2
        {
            Deck = DaveDeck.Deck,
            Description = AnyLocalizations.Bind(["character", "description"]).Localize,
            BorderSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/char_frame_dave.png")).Sprite,
            NeutralAnimation = new CharacterAnimationConfigurationV2
            {
                LoopTag = "neutral",
                Frames = Enumerable.Range(0,
                        3)
                    .Select(i =>
                        helper.Content.Sprites
                            .RegisterSprite(
                                package.PackageRoot.GetRelativeFile($"Sprites/Animation/DaveNeutral{i}.png"))
                            .Sprite)
                    .ToList(),
                CharacterType = DaveDeck.Deck.Key()
            },
            MiniAnimation = new CharacterAnimationConfigurationV2
            {
                LoopTag = "mini",
                Frames =
                [
                    helper.Content.Sprites
                        .RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Animation/DaveMini.png")).Sprite
                ],
                CharacterType = DaveDeck.Deck.Key()
            },
            Starters = new StarterDeck
            {
                cards =
                [
                    new LuckyEscapeCard(),
                    new RiggingShotCard()
                ],
                artifacts = [
                    new Chip()
                ]
            }
        });

        helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2
        {
            LoopTag = "gameover",
            Frames = [
                helper.Content.Sprites
                    .RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Animation/DaveGameOver.png")).Sprite
            ],
            CharacterType = DaveDeck.Deck.Key()
        });
        helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2
        {
            LoopTag = "squint",
            Frames = Enumerable.Range(0, 3)
                .Select(i => helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"Sprites/Animation/DaveSquint{i}.png")).Sprite)
                .ToList(),
            CharacterType = DaveDeck.Deck.Key()
        });
        
        MoreDifficultiesApi?.RegisterAltStarters(
            DaveDeck.Deck,
            new StarterDeck
            {
                cards =
                [
                    new WildShotCard(),
                    new WildStepCard()
                ],
                artifacts = [
                    new Chip()
                ]
            }
        );
        
        // status registration
        RedRigging = helper.Content.Statuses.RegisterStatus("RedRigging", new StatusConfiguration
        {
            Definition = new StatusDef
            {
                icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/red_chip.png")).Sprite,
                color = new Color("FF0000"),
                isGood = true
            },
            Name = AnyLocalizations.Bind(["status", "RedRigging", "name"]).Localize,
            Description = AnyLocalizations.Bind(["status", "RedRigging", "description"]).Localize
        });
        BlackRigging = helper.Content.Statuses.RegisterStatus("BlackRigging", new StatusConfiguration
        {
            Definition = new StatusDef
            {
                icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/black_chip.png")).Sprite,
                color = new Color("000000"),
                isGood = true
            },
            Name = AnyLocalizations.Bind(["status", "BlackRigging", "name"]).Localize,
            Description = AnyLocalizations.Bind(["status", "BlackRigging", "description"]).Localize
        });
        RedBias = helper.Content.Statuses.RegisterStatus("RedBias", new StatusConfiguration
        {
            Definition = new StatusDef
            {
                icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/red_clover.png")).Sprite,
                color = new Color("FF0000"),
                isGood = true
            },
            Name = AnyLocalizations.Bind(["status", "RedBias", "name"]).Localize,
            Description = AnyLocalizations.Bind(["status", "RedBias", "description"]).Localize
        });
        BlackBias = helper.Content.Statuses.RegisterStatus("BlackBias", new StatusConfiguration
        {
            Definition = new StatusDef
            {
                icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/black_clover.png")).Sprite,
                color = new Color("000000"),
                isGood = true
            },
            Name = AnyLocalizations.Bind(["status", "BlackBias", "name"]).Localize,
            Description = AnyLocalizations.Bind(["status", "BlackBias", "description"]).Localize
        });
        BiasStatusAction.SprRed = RedBias.Configuration.Definition.icon;
        BiasStatusAction.SprBlack = BlackBias.Configuration.Definition.icon;
        
        // artifact registration
        helper.Content.Artifacts.RegisterArtifact("Chip", new ArtifactConfiguration
        {
            Name = AnyLocalizations.Bind(["artifact", "Chip", "name"]).Localize,
            Description = AnyLocalizations.Bind(["artifact", "Chip", "description"]).Localize,
            ArtifactType = typeof(Chip),
            Meta = new ArtifactMeta
            {
                owner = DaveDeck.Deck,
                pools = [ArtifactPool.Common],
                unremovable = true
            },
            Sprite = 0
        });
        Chip.Neutral = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Chip_neutral.png")).Sprite;
        Chip.Red = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Chip_red.png")).Sprite;
        Chip.Black = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Chip_black.png")).Sprite;
        Chip.Both = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Chip_both.png")).Sprite;

        helper.Content.Artifacts.RegisterArtifact("DriveRefund", new ArtifactConfiguration
        {
            Name = AnyLocalizations.Bind(["artifact", "DriveRefund", "name"]).Localize,
            Description = AnyLocalizations.Bind(["artifact", "DriveRefund", "description"]).Localize,
            ArtifactType = typeof(DriveRefund),
            Meta = new ArtifactMeta
            {
                owner = DaveDeck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = 0
        });
        DriveRefund.On = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/DriveRefund.png")).Sprite;
        DriveRefund.Off = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/DriveRefund_off.png")).Sprite;

        helper.Content.Artifacts.RegisterArtifact("Ashtray", new ArtifactConfiguration
        {
            Name = AnyLocalizations.Bind(["artifact", "Ashtray", "name"]).Localize,
            Description = AnyLocalizations.Bind(["artifact", "Ashtray", "description"]).Localize,
            ArtifactType = typeof(Ashtray),
            Meta = new ArtifactMeta
            {
                owner = DaveDeck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Ashtray.png")).Sprite
        });

        helper.Content.Artifacts.RegisterArtifact("Roulette", new ArtifactConfiguration
        {
            Name = AnyLocalizations.Bind(["artifact", "Roulette", "name"]).Localize,
            Description = AnyLocalizations.Bind(["artifact", "Roulette", "description"]).Localize,
            ArtifactType = typeof(Roulette),
            Meta = new ArtifactMeta
            {
                owner = DaveDeck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Roulette.png")).Sprite
        });

        helper.Content.Artifacts.RegisterArtifact("UnderdriveGenerator", new ArtifactConfiguration
        {
            Name = AnyLocalizations.Bind(["artifact", "UnderdriveGenerator", "name"]).Localize,
            Description = AnyLocalizations.Bind(["artifact", "UnderdriveGenerator", "description"]).Localize,
            ArtifactType = typeof(UnderdriveGenerator),
            Meta = new ArtifactMeta
            {
                owner = DaveDeck.Deck,
                pools = [ArtifactPool.Boss]
            },
            Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/UnderdriveGenerator.png")).Sprite
        });

        helper.Content.Artifacts.RegisterArtifact("RiggedDice", new ArtifactConfiguration
        {
            Name = AnyLocalizations.Bind(["artifact", "RiggedDice", "name"]).Localize,
            Description = AnyLocalizations.Bind(["artifact", "RiggedDice", "description"]).Localize,
            ArtifactType = typeof(RiggedDice),
            Meta = new ArtifactMeta
            {
                owner = DaveDeck.Deck,
                pools = [ArtifactPool.Boss]
            },
            Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/RiggedDice.png")).Sprite
        });
        
        // other registration
        RedBlackCondition.Red = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/red.png")).Sprite;
        RedBlackCondition.Black = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/black.png")).Sprite;
        RandomMoveFoeAction.Spr = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/random_move_foe.png")).Sprite;
        ShieldHurtAction.Spr = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/shield_hurt.png")).Sprite;
        
        // post registration
        helper.Events.OnModLoadPhaseFinished += (_, phase) =>
        {
            if (phase != ModLoadPhase.AfterDbInit) return;
            if (DraculaApi == null) return;

            DraculaApi.RegisterBloodTapOptionProvider(new BloodTapProvider());
        };
        
        Harmony.PatchAll();
        
        JesterApi?.RegisterProvider(new DaveJesterProvider());
        JesterApi?.RegisterStrategy(new DaveJesterStrategy());
        JesterApi?.RegisterCharacterFlag("dave_rigging", DaveDeck.Deck);
        
        // duo registration
        helper.ModRegistry.AwaitApi<IDuoApi>("Shockah.DuoArtifacts", api =>
        {
            foreach (var artifactType in DuoArtifacts)
                AccessTools.DeclaredMethod(artifactType, nameof(IDuoArtifact.Register))?.Invoke(null, [package, helper, api]);
        });
    }

    private void RegisterCard(IPluginPackage<IModManifest> package, IModHelper helper, Type type, string locName, bool offer = true)
    {
        helper.Content.Cards.RegisterCard(type.Name, new CardConfiguration
        {
            CardType = type,
            Meta = new CardMeta
            {
                deck = DaveDeck.Deck,
                rarity = GetCardRarity(type),
                upgradesTo = [Upgrade.A, Upgrade.B],
                dontOffer = !offer
            },
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"Sprites/Card/{locName}.png")).Sprite,
            Name = AnyLocalizations.Bind(["card", locName, "name"]).Localize
        });
    }

    private static Rarity GetCardRarity(Type type)
    {
        if (RareCards.Contains(type)) return Rarity.rare;
        if (UncommonCards.Contains(type)) return Rarity.uncommon;
        return Rarity.common;
    }
}