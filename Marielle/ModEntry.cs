using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Marielle.Artifacts;
using Marielle.Cards;
using Marielle.ExternalAPI;
using Marielle.Features;
using Marielle.Jester;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using IJesterApi = Marielle.ExternalAPI.IJesterApi;

/* In the Cobalt Core modding community it is common for namespaces to be <Author>.<ModName>
 * This is helpful to know at a glance what mod we're looking at, and who made it */
namespace Marielle;

/* ModEntry is the base for our mod. Others like to name it Manifest, and some like to name it <ModName>
 * Notice the ': SimpleMod'. This means ModEntry is a subclass (child) of the superclass SimpleMod (parent) from Nickel. This will help us use Nickel's functions more easily! */
public sealed class ModEntry : SimpleMod
{
    internal static ModEntry Instance { get; private set; } = null!;
    internal IKokoroApi KokoroApi { get; }
    internal ILouisApi? LouisApi { get; }
    internal IJesterApi? JesterApi { get; }
    internal IMoreDifficultiesApi? MoreDifficultyApi { get; }
    internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
    internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }
    internal IDeckEntry MarielleDeck { get; }
    internal IStatusEntry Curse { get; }
    internal IStatusEntry Enflamed { get; } 
    internal ICardTraitEntry Fleeting { get; }

    /* You can create many IReadOnlyList<Type> as a way to organize your content.
     * We recommend having a Starter Cards list, a Common Cards list, an Uncommon Cards list, and a Rare Cards list
     * However you can be more detailed, or you can be more loose, if that's your style */
    internal static IReadOnlyList<Type> MarielleCommonCardTypes { get; } = [
        typeof(FireField),
        typeof(Hex),
        typeof(Blowtorch),
        typeof(HotShot),
        typeof(Prayer),
        typeof(Voodoo),
        typeof(HeatRay),
        typeof(WickedPact),
        typeof(ShadowCover)
    ];

    internal static IReadOnlyList<Type> MarielleUncommonCardTypes { get; } = [
        typeof(Immolation),
        typeof(BlessingAndACurse),
        typeof(Balance),
        typeof(MagnesiumSlug),
        typeof(Brimstone),
        typeof(BurningRage),
        typeof(Malediction)
    ];
    
    internal static IReadOnlyList<Type> MarielleRareCardTypes { get; } = [
        typeof(FanTheFlames),
        typeof(RendAsunder),
        typeof(Scourge),
        typeof(SlowRoast),
        typeof(DarkThoughts)
    ];

    internal static IReadOnlyList<Type> MarielleArtifactCardTypes { get; } = [
        typeof(Absolve)
    ];

    /* We can use an IEnumerable to combine the lists we made above, and modify it if needed
     * Maybe you created a new list for Uncommon cards, and want to add it.
     * If so, you can .Concat(TheUncommonListYouMade) */
    internal static IEnumerable<Type> MarielleAllCardTypes
        => MarielleCommonCardTypes
            .Concat(MarielleUncommonCardTypes)
            .Concat(MarielleRareCardTypes)
            .Concat(MarielleArtifactCardTypes);

    /* We'll organize our artifacts the same way: making lists and then feed those to an IEnumerable */
    internal static IReadOnlyList<Type> MarielleCommonArtifactTypes { get; } = [
        typeof(Repentance),
        typeof(DarkAura),
        typeof(ColdHearted),
        typeof(HotPotato),
        typeof(RedCandle),
        typeof(TollingBell)
    ];

    internal static IReadOnlyList<Type> MarielleBossArtifactTypes { get; } = [
        typeof(NoMercy)
    ];
    
    internal static IEnumerable<Type> MarielleAllArtifactTypes
        => MarielleCommonArtifactTypes
        .Concat(MarielleBossArtifactTypes)
        .Append(typeof(VanillaDuos));


    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
    {
        Instance = this;

        /* We use Kokoro to handle our statuses. This means Kokoro is a Dependency, and our mod will fail to work without it.
         * We take from Kokoro what we need and put in our own project. Head to ExternalAPI/StatusLogicHook.cs if you're interested in what, exactly, we use.
         * If you're interested in more fancy stuff, make sure to peek at the Kokoro repository found online. */
        KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!;
        LouisApi = helper.ModRegistry.GetApi<ILouisApi>("TheJazMaster.Louis");
        JesterApi = helper.ModRegistry.GetApi<IJesterApi>("rft.Jester");
        MoreDifficultyApi = helper.ModRegistry.GetApi<IMoreDifficultiesApi>("TheJazMaster.MoreDifficulties");

        /* These localizations lists help us organize our mod's text and messages by language.
         * For general use, prefer AnyLocalizations, as that will provide an easier time to potential localization submods that are made for your mod 
         * IMPORTANT: These localizations are found in the i18n folder (short for internationalization). The Demo Mod comes with a barebones en.json localization file that you might want to check out before continuing 
         * Whenever you add a card, artifact, character, ship, pretty much whatever, you will want to update your locale file in i18n with the necessary information
         * Example: You added your own character, you will want to create an appropiate entry in the i18n file. 
         * If you would rather use simple strings whenever possible, that's also an option -you do you. */
        AnyLocalizations = new JsonLocalizationProvider(
            tokenExtractor: new SimpleLocalizationTokenExtractor(),
            localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"i18n/{locale}.json").OpenRead()
        );
        Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
            new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(AnyLocalizations)
        );

        /* Decks are assigned separate of the character. This is because the game has decks like Trash which is not related to a playable character
         * Do note that Color accepts a HEX string format (like Color("a1b2c3")) or a Float RGB format (like Color(0.63, 0.7, 0.76). It does NOT allow a traditional RGB format (Meaning Color(161, 178, 195) will NOT work) */
        MarielleDeck = helper.Content.Decks.RegisterDeck("Marielle", new DeckConfiguration
        {
            Definition = new DeckDef
            {
                /* This color is used in various situations. 
                 * It is used as the deck's rarity 'shine'
                 * If a playable character uses this deck, the character Name will use this color
                 * If a playable character uses this deck, the character mini panel will use this color */
                color = new Color("ac507e"),

                /* This color is for the card name in-game
                 * Make sure it has a good contrast against the CardFrame, and take rarity 'shine' into account as well */
                titleColor = new Color("000000")
            },
            /* We give it a default art and border some Sprite types by adding '.Sprite' at the end of the ISpriteEntry definitions we made above. */
            DefaultCardArt = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/DefaultArt.png")).Sprite,
            BorderSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/CardBase.png")).Sprite,

            /* Since this deck will be used by our Demo Character, we'll use their name. */
            Name = AnyLocalizations.Bind(["character", "Marielle", "name"]).Localize,
        });

        /* Let's create some animations, because if you were to boot up this mod from what you have above,
         * DemoCharacter would be a blank void inside a box, we haven't added their sprites yet! 
         * We first begin by registering the animations. I know, weird. 'Why are we making animations when we still haven't made the character itself', stick with me, okay? 
         * Animations are actually assigned to Deck types, not Characters! */

        /*Of Note: You may notice we aren't assigning these ICharacterAnimationEntry and ICharacterEntry to any object, unlike stuff above,
        * It's totally fine to assign them if you'd like, but we don't have a reason to so in this mod */
        helper.Content.Characters.RegisterCharacterAnimation(new CharacterAnimationConfiguration
        {
            /* What we registered above was an IDeckEntry object, but when you register a character animation the Helper will ask for you to provide its Deck 'id'
             * This is simple enough, as you can get it from DemoMod_Deck */
            Deck = MarielleDeck.Deck,

            /* The Looptag is the 'name' of the animation. When making shouts and events, and you want your character to show emotions, the LoopTag is what you want
             * In vanilla Cobalt Core, there are 4 'animations' looptags that any character should have: "neutral", "mini", "squint" and "gameover",
             * as these are used in: Neutral is used as default, mini is used in character select and out-of-combat UI, Squink is hardcoded used in certain events, and Gameover is used when your run ends */
            LoopTag = "neutral",

            /* The game doesn't use frames properly when there are only 2 or 3 frames. If you want a proper animation, avoid only adding 2 or 3 frames to it */
            Frames = new[]
            {
                helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Marielle/Marielle_neutral_0.png")).Sprite,
                helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Marielle/Marielle_neutral_1.png")).Sprite,
                helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Marielle/Marielle_neutral_2.png")).Sprite,
                helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Marielle/Marielle_neutral_3.png")).Sprite
            }
        });
        helper.Content.Characters.RegisterCharacterAnimation(new CharacterAnimationConfiguration
        {
            Deck = MarielleDeck.Deck,
            LoopTag = "mini",
            Frames = new[]
            {
                helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Marielle/Mini_marielle.png")).Sprite
            }
        });
        helper.Content.Characters.RegisterCharacterAnimation(new CharacterAnimationConfiguration
        {
            Deck = MarielleDeck.Deck,
            LoopTag = "squint",
            Frames = new[]
            {
                helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Marielle/Marielle_squinting_0.png")).Sprite,
                helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Marielle/Marielle_squinting_1.png")).Sprite,
                helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Marielle/Marielle_squinting_2.png")).Sprite,
                helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Marielle/Marielle_squinting_3.png")).Sprite
            }
        });
        helper.Content.Characters.RegisterCharacterAnimation(new CharacterAnimationConfiguration
        {
            Deck = MarielleDeck.Deck,
            LoopTag = "gameover",
            Frames = new[]
            {
                helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Marielle/Marielle_death.png")).Sprite
            }
        });
        
        /* Let's continue with the character creation and finally, actually, register the character! */
        helper.Content.Characters.RegisterCharacter("Marielle", new CharacterConfiguration()
        {
            /* Same as animations, we want to provide the appropiate Deck type */
            Deck = MarielleDeck.Deck,

            /* The Starter Card Types are, as the name implies, the cards you will start a DemoCharacter run with. 
             * You could provide vanilla cards if you want, but it's way more fun to create your own cards! */
            Starters = new StarterDeck
            {
                cards = new List<Card>
                {
                    new FireField(),
                    new Hex()
                }
            },

            /* This is the little blurb that appears when you hover over the character in-game.
             * You can make it fluff, use it as a way to tell players about the character's playstyle, or a little bit of both! */
            Description = AnyLocalizations.Bind(["character", "Marielle", "description"]).Localize,

            /* This is the fancy panel that encapsulates your character while in active combat.
             * It's recommended that it follows the same color scheme as the character and deck, for cohesion */
            BorderSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Marielle/Panel.png")).Sprite
        });
        
        MoreDifficultyApi?.RegisterAltStarters(MarielleDeck.Deck, new StarterDeck
        {
            cards = new List<Card>
            {
                new Blowtorch(),
                new ShadowCover()
            }
        });

        /* The basics for a Character mod are done!
         * But you may still have mechanics you want to tackle, such as,
         * 1. How to make cards
         * 2. How to make artifacts
         * 4. How to make statuses */

        /* 1. CARDS
         * DemoMod comes with a neat folder called Cards where all the .cs files for our cards are stored. Take a look.
         * You can decide to not use the folder, or to add more folders to further organize your cards. That is up to you.
         * We do recommend keeping files organized, however. It's way easier to traverse a project when the paths are clear and meaningful */

        /* Here we register our cards so we can find them in game.
         * Notice the IDemoCard interface, you can find it in InternalInterfaces.cs
         * Each card in the IEnumerable 'DemoMod_AllCard_Types' will be asked to run their 'Register' method. Open a card's .cs file, and see what it does 
         * We *can* instead register characts one by one, like what we did with the sprites. If you'd like an example of what that looks like, check out the Randall mod by Arin! */
        foreach (var cardType in MarielleAllCardTypes)
            AccessTools.DeclaredMethod(cardType, nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);

        /* 2. ARTIFACTS
         * Creating artifacts is pretty similar to creating Cards
         * Take a look at the Artifacts folder for demo artifacts!
         * You may also notice we're using the other interface from InternalInterfaces.cs, IDemoArtifact, to help us out */
        foreach (var artifactType in MarielleAllArtifactTypes)
            AccessTools.DeclaredMethod(artifactType, nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);

        /* 4. STATUSES
         * You might, now, with all this code behind our backs, start recognizing patterns in the way we can register stuff. */
        Curse = helper.Content.Statuses.RegisterStatus("Curse", new()
        {
            Definition = new StatusDef
            {
                icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Statuses/Curse.png")).Sprite,
                color = new("b500be"),
                isGood = false
            },
            Name = AnyLocalizations.Bind(["status", "Curse", "name"]).Localize,
            Description = AnyLocalizations.Bind(["status", "Curse", "description"]).Localize
        });
        Enflamed = helper.Content.Statuses.RegisterStatus("Enflamed", new()
        {
            Definition = new StatusDef
            {
                icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Statuses/Enflamed.png")).Sprite,
                color = new("b500be"),
                isGood = false
            },
            Name = AnyLocalizations.Bind(["status", "Enflamed", "name"]).Localize,
            Description = AnyLocalizations.Bind(["status", "Enflamed", "description"]).Localize
        });
        _ = new StatusManager();
        
        /* 5. TRAITS */
        {
            if (LouisApi != null)
            {
                Fleeting = LouisApi.FleetingTrait;
                TraitManager.HandleFleeting = false;
            }
            else
            {
                var sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Statuses/Fleeting.png")).Sprite;
                Fleeting = helper.Content.Cards.RegisterTrait("Fleeting", new()
                {
                    Name = AnyLocalizations.Bind(["trait", "Fleeting", "name"]).Localize,
                    Icon = (_, _) => sprite,
                    Tooltips = (_, _) => [
                        new GlossaryTooltip($"trait.{GetType().Namespace!}::Fleeting")
                        {
                            Icon = sprite,
                            TitleColor = Colors.action,
                            Title = Localizations.Localize(["trait", "Fleeting", "name"]),
                            Description = Localizations.Localize(["trait", "Fleeting", "description"])
                        }
                    ]
                });
                TraitManager.HandleFleeting = true;
            }
        }
        _ = new TraitManager();
        
        /* 6. HARMONY */
        var harmony = new Harmony("rft.Marielle");
        harmony.PatchAll();
        
        /* 7. JESTER */
        JesterApi?.RegisterCharacterFlag("heat", MarielleDeck.Deck);
        JesterApi?.RegisterCharacterFlag("enemyHeat", MarielleDeck.Deck);
        JesterApi?.RegisterProvider(new MarielleJesterProvider());
    }

    public override object GetApi(IModManifest requestingMod) => new MarielleApi();
}

public sealed class MarielleApi : IMarielleApi
{
    public IDeckEntry MarielleDeck() => ModEntry.Instance.MarielleDeck;

    public IStatusEntry Curse() => ModEntry.Instance.Curse;

    public IStatusEntry Enflamed() => ModEntry.Instance.Enflamed;

    public ICardTraitEntry Fleeting() => ModEntry.Instance.Fleeting;
}