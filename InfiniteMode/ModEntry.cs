using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using InfiniteMode.Artifacts;
using InfiniteMode.Features;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using IKokoroApi = InfiniteMode.ExternalAPI.IKokoroApi;

/* In the Cobalt Core modding community it is common for namespaces to be <Author>.<ModName>
 * This is helpful to know at a glance what mod we're looking at, and who made it */
namespace InfiniteMode;

/* ModEntry is the base for our mod. Others like to name it Manifest, and some like to name it <ModName>
 * Notice the ': SimpleMod'. This means ModEntry is a subclass (child) of the superclass SimpleMod (parent) from Nickel. This will help us use Nickel's functions more easily! */
public sealed class ModEntry : SimpleMod
{
    internal static ModEntry Instance { get; private set; } = null!;
    internal IKokoroApi KokoroApi { get; }
    internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
    internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }
    internal ICardTraitEntry CorruptedCardTrait { get; }

    /* We'll organize our artifacts the same way: making lists and then feed those to an IEnumerable */
    internal static IReadOnlyList<Type> TrackingArtifacts { get; } = [
        typeof(InfinityArtifact)
    ];

    internal static IEnumerable<Type> AllArtifacts
        => TrackingArtifacts
            .Concat([]);


    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
    {
        Instance = this;

        /* We use Kokoro to handle our statuses. This means Kokoro is a Dependency, and our mod will fail to work without it.
         * We take from Kokoro what we need and put in our own project. Head to ExternalAPI/StatusLogicHook.cs if you're interested in what, exactly, we use.
         * If you're interested in more fancy stuff, make sure to peek at the Kokoro repository found online. */
        KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!;
        KokoroApi.RegisterTypeForExtensionData(typeof(State));
        KokoroApi.RegisterTypeForExtensionData(typeof(Combat));
        KokoroApi.RegisterTypeForExtensionData(typeof(Artifact));

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

        /* The basics for a Character mod are done!
         * But you may still have mechanics you want to tackle, such as,
         * 2. How to make artifacts
         * 4. How to make statuses */

        /* 2. ARTIFACTS
         * Creating artifacts is pretty similar to creating Cards
         * Take a look at the Artifacts folder for demo artifacts!
         * You may also notice we're using the other interface from InternalInterfaces.cs, IDemoArtifact, to help us out */
        foreach (var artifactType in AllArtifacts)
            AccessTools.DeclaredMethod(artifactType, nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);
        
        
        /* 5. TRAITS */
        {
            
            var sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Statuses/MrStarkIDontFeelSoGood2.png")).Sprite;
            CorruptedCardTrait = helper.Content.Cards.RegisterTrait("CorruptedCard", new()
            {
                Name = AnyLocalizations.Bind(["trait", "CorruptedCard", "name"]).Localize,
                Icon = (_, _) => sprite,
                Tooltips = (_, _) => [
                    new GlossaryTooltip($"trait.{GetType().Namespace!}::CorruptedCard")
                    {
                        Icon = sprite,
                        TitleColor = Colors.action,
                        Title = Localizations.Localize(["trait", "CorruptedCard", "name"]),
                        Description = Localizations.Localize(["trait", "CorruptedCard", "description"])
                    }
                ]
            });
        }
        
        /* 6. HARMONY */
        var harmony = new Harmony("rft.InfiniteMode");
        harmony.PatchAll();
        
        /* 7. MANAGERS */
        RestartOptionManager.Instance.Register();
        CorruptedCardManager.Instance.Register(helper);
        CorruptedArtifactManager.Instance.Register();
    }
}