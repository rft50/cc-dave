using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Marielle.ExternalAPI;
using Nanoray.PluginManager;
using Nickel;

namespace Marielle.Artifacts;

public class ModdedDuos : IRegisterable
{
    private static readonly List<Type> ArtifactTypes =
    [
        typeof(MarielleTySashaDuoArtifact),
        typeof(MarielleJohnsonDuoArtifact),
        typeof(MarielleBucketDuoArtifact),
        typeof(MarielleTH34DuoArtifact),
        typeof(MarielleDynaDuoArtifact)
    ];
    
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.ModRegistry.AwaitApi<IDuoApi>("Shockah.DuoArtifacts", api =>
        {
            foreach (var artifactType in ArtifactTypes)
            {
                AccessTools.DeclaredMethod(artifactType, nameof(IDuoArtifact.Register))?.Invoke(null, [package, helper, api]);
            }
        });
    }
}

public class MarielleTySashaDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.ModRegistry.AwaitApi<ITyAndSashaApi>("TheJazMaster.TyAndSasha", api =>
        {
            helper.Content.Artifacts.RegisterArtifact(nameof(MarielleTySashaDuoArtifact), new()
            {
                ArtifactType = typeof(MarielleTySashaDuoArtifact),
                Meta = new()
                {
                    owner = duoApi.DuoArtifactVanillaDeck,
                    pools = [ArtifactPool.Common]
                },
                Sprite = helper.Content.Sprites
                    .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/Duo/CollarBell.png")).Sprite,
                Name = ModEntry.Instance.AnyLocalizations.Bind(["duo", "TySasha", "name"]).Localize,
                Description = ModEntry.Instance.AnyLocalizations.Bind(["duo", "TySasha", "description"]).Localize
            });
            duoApi.RegisterDuoArtifact(typeof(MarielleTySashaDuoArtifact), [ModEntry.Instance.MarielleDeck.Deck, api.TyDeck]);
            
            // Register the hook with TyAndSasha API
            api.RegisterHook(new TySashaCompanionHook());
        });
    }
    
    private class TySashaCompanionHook : ITyAndSashaApi.IHook
    {
        public bool ApplyXBonus(ITyAndSashaApi.IHook.IApplyXBonusArgs args)
        {
            return false;
        }

        public int AffectX(ITyAndSashaApi.IHook.IAffectXArgs args)
        {
            // Check if the Marielle-TySasha duo artifact is present
            if (!args.State.EnumerateAllArtifacts().Any(a => a is MarielleTySashaDuoArtifact))
                return 0;
                
            // Check if the player is overheating (heat >= heatTrigger)
            if (args.State.ship.Get(Status.heat) < args.State.ship.heatTrigger)
                return 0;
                
            // Return the amount of curse the player has as X bonus
            return args.State.ship.Get(ModEntry.Instance.Curse.Status);
        }
    }
}

public class MarielleJohnsonDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.ModRegistry.AwaitApi<IJohnsonApi>("Shockah.Johnson", api =>
        {
            helper.Content.Artifacts.RegisterArtifact(nameof(MarielleJohnsonDuoArtifact), new()
            {
                ArtifactType = typeof(MarielleJohnsonDuoArtifact),
                Meta = new()
                {
                    owner = duoApi.DuoArtifactVanillaDeck,
                    pools = [ArtifactPool.Common]
                },
                Sprite = helper.Content.Sprites
                    .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/Duo/LegalBinding.png")).Sprite,
                Name = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Johnson", "name"]).Localize,
                Description = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Johnson", "description"]).Localize
            });
            duoApi.RegisterDuoArtifact(typeof(MarielleJohnsonDuoArtifact), [ModEntry.Instance.MarielleDeck.Deck, api.JohnsonDeck.Deck]);
        });
    }
    
    public override void OnPlayerPlayCard(
        int energyCost,
        Deck deck,
        Card card,
        State state,
        Combat combat,
        int handPosition,
        int handCount)
    {
        if (ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(state, card,
                ModEntry.Instance.Helper.Content.Cards.TemporaryCardTrait))
        {
            // Apply 1 heat to the enemy
            combat.QueueImmediate(new AStatus
            {
                status = Status.heat,
                statusAmount = 1,
                targetPlayer = false,
                artifactPulse = Key()
            });
        }
    }
}

[HarmonyPatch]
public class MarielleBucketDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.ModRegistry.AwaitApi<IBucketApi>("TheJazMaster.Bucket", api =>
        {
            helper.Content.Artifacts.RegisterArtifact(nameof(MarielleBucketDuoArtifact), new()
            {
                ArtifactType = typeof(MarielleBucketDuoArtifact),
                Meta = new()
                {
                    owner = duoApi.DuoArtifactVanillaDeck,
                    pools = [ArtifactPool.Common]
                },
                Sprite = helper.Content.Sprites
                    .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/Duo/Temperance.png")).Sprite,
                Name = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Bucket", "name"]).Localize,
                Description = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Bucket", "description"]).Localize
            });
            duoApi.RegisterDuoArtifact(typeof(MarielleBucketDuoArtifact), [ModEntry.Instance.MarielleDeck.Deck, api.BucketDeck]);
        });
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(AAddCard), nameof(AAddCard.Begin))]
    public static bool AAddCard_Begin_Prefix(AAddCard __instance, State s, Combat c)
    {
        // Check if we have a MarielleBucketDuoArtifact in play
        if (!s.EnumerateAllArtifacts().Any(a => a is MarielleBucketDuoArtifact))
            return true; // Continue with original method if we don't have the artifact
            
        // Check if the player has Serenity status
        var serenity = s.ship.Get(Status.serenity);
        if (serenity <= 0)
            return true; // Continue with original method if no Serenity
            
        // Check if the card being added is a trash card
        if (__instance.card.GetMeta().deck == Deck.trash)
        {
            // Prevent adding trash cards if we have Serenity
            // Pulse the artifact to show it activated
            s.artifacts.OfType<MarielleBucketDuoArtifact>().FirstOrDefault()?.Pulse();

            c.QueueImmediate(new AStatus
            {
                status = Status.serenity,
                targetPlayer = true,
                statusAmount = -1,
            });
            return false;
        }
        
        // Continue with the original method for non-trash cards
        return true;
    }
}


// ReSharper disable once InconsistentNaming
public class MarielleTH34DuoArtifact : Artifact, IDuoArtifact
{
    public int TurnCounter = 0;
    private static IStatusEntry _refractoryStatus = null!;
    
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.ModRegistry.AwaitApi<ITH34Api>("FredsTH34", api =>
        {
            helper.Content.Artifacts.RegisterArtifact(nameof(MarielleTH34DuoArtifact), new()
            {
                ArtifactType = typeof(MarielleTH34DuoArtifact),
                Meta = new()
                {
                    owner = duoApi.DuoArtifactVanillaDeck,
                    pools = [ArtifactPool.Common]
                },
                Sprite = helper.Content.Sprites
                    .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/Duo/BlackMetal.png")).Sprite,
                Name = ModEntry.Instance.AnyLocalizations.Bind(["duo", "TH34", "name"]).Localize,
                Description = ModEntry.Instance.AnyLocalizations.Bind(["duo", "TH34", "description"]).Localize
            });
            duoApi.RegisterDuoArtifact(typeof(MarielleTH34DuoArtifact), [ModEntry.Instance.MarielleDeck.Deck, api.TH34_Deck.Deck]);
            _refractoryStatus = api.RefractoryStatus;
        });
    }
    
    public override List<Tooltip> GetExtraTooltips() =>
    [
        StatusMeta.GetTooltips(ModEntry.Instance.Curse.Status, 1)[0],
        StatusMeta.GetTooltips(_refractoryStatus.Status, 1)[0]
    ];
    
    public override int? GetDisplayNumber(State s)
    {
        return TurnCounter;
    }
    
    public override void OnTurnStart(State state, Combat combat)
    {
        TurnCounter++;

        if (TurnCounter < 2) return;
        TurnCounter -= 2;
        
        if (state.ship.Get(ModEntry.Instance.Curse.Status) <= 0) return;
        
        combat.Queue([
            new AStatus
            {
                status = ModEntry.Instance.Curse.Status,
                statusAmount = -1,
                targetPlayer = true,
                timer = 0
            },
            new AStatus
            {
                status = _refractoryStatus.Status,
                statusAmount = 1,
                targetPlayer = true,
                artifactPulse = Key()
            }
        ]);
    }
}

[HarmonyPatch]
public class MarielleDynaDuoArtifact : Artifact, IDuoArtifact
{
    private static IDynaApi _dynaApi = null!;
    
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.ModRegistry.AwaitApi<IDynaApi>("Shockah.Dyna", api =>
        {
            helper.Content.Artifacts.RegisterArtifact(nameof(MarielleDynaDuoArtifact), new()
            {
                ArtifactType = typeof(MarielleDynaDuoArtifact),
                Meta = new()
                {
                    owner = duoApi.DuoArtifactVanillaDeck,
                    pools = [ArtifactPool.Common]
                },
                Sprite = helper.Content.Sprites
                    .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/Duo/InsultToInjury.png")).Sprite,
                Name = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Dyna", "name"]).Localize,
                Description = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Dyna", "description"]).Localize
            });
            duoApi.RegisterDuoArtifact(typeof(MarielleDynaDuoArtifact), [ModEntry.Instance.MarielleDeck.Deck, api.DynaDeck.Deck]);
            _dynaApi = api;
        });
    }
    
    public override List<Tooltip> GetExtraTooltips() =>
    [
        new TTGlossary("status.heat", 1)
    ];
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Ship), "NormalDamage")]
    public static void Ship_NormalDamage_Postfix(
        Ship __instance, 
        State s, 
        Combat c,
        int? maybeWorldGridX, 
        DamageDone __result)
    {
        // Only apply when enemy ship is being damaged
        if (__instance.isPlayerShip || !__result.hitHull)
            return;
        
        // Check if the MarielleDynaDuoArtifact is in play
        var artifact = s.EnumerateAllArtifacts().FirstOrDefault(a => a is MarielleDynaDuoArtifact);
        if (artifact == null)
            return;
            
        var partAtWorldX = !maybeWorldGridX.HasValue ? null : __instance.GetPartAtWorldX(maybeWorldGridX.GetValueOrDefault());
        var partMod = partAtWorldX?.damageModifier;
        if (partMod == null || !(partMod == PDamMod.weak
                                 || partMod == PDamMod.brittle
                                 || partMod == _dynaApi.FluxDamageModifier))
            return;
        
        // Apply heat to enemy ship when it takes hull damage
        c.QueueImmediate(new AStatus
        {
            status = Status.heat,
            statusAmount = 1,
            targetPlayer = false,
            artifactPulse = artifact.Key()
        });
    }
}
