using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Marielle.ExternalAPI;
using Nanoray.PluginManager;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Nickel;

namespace Marielle.Artifacts;

public interface IDuoArtifact
{
    public static abstract void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi);
}

public class VanillaDuos : IRegisterable
{
    private static readonly List<Type> ArtifactTypes =
    [
        typeof(MarielleRiggsDuoArtifact),
        typeof(MarielleDizzyDuoArtifact),
        typeof(MariellePeriDuoArtifact),
        typeof(MarielleIsaacDuoArtifact),
        typeof(MarielleDrakeDuoArtifact),
        typeof(MarielleMaxDuoArtifact),
        typeof(MarielleBooksDuoArtifact),
        typeof(MarielleCatDuoArtifact)
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

public class MarielleRiggsDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/Duo/WaxWings.png")).Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Riggs", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Riggs", "description"]).Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.MarielleDeck.Deck, Deck.riggs]);
    }

    public override List<Tooltip> GetExtraTooltips() =>
    [
        new TTGlossary("status.evade", 1),
        new TTGlossary("status.heat", 1)
    ];

    public override void OnTurnStart(State state, Combat combat)
    {
        if (state.ship.Get(Status.heat) <= state.ship.heatTrigger - 2)
        {
            combat.Queue([
                new AStatus
                {
                    status = Status.evade,
                    statusAmount = 1,
                    targetPlayer = true,
                    timer = 0
                },
                new AStatus
                {
                    status = Status.heat,
                    statusAmount = 1,
                    targetPlayer = true,
                    artifactPulse = Key()
                }
            ]);
        }
    }
}

[HarmonyPatch]
public class MarielleDizzyDuoArtifact : Artifact, IDuoArtifact
{
    private static bool _duringOverheat;
    public int Charges = 2;
    
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/Duo/DivineIntervention.png")).Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Dizzy", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Dizzy", "description"]).Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.MarielleDeck.Deck, Deck.dizzy]);
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        Charges = 2;
    }

    public override int? GetDisplayNumber(State s) => Charges;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AOverheat), "Begin")]
    private static void AOverheat_Begin_Prefix() => _duringOverheat = true;
    
    [HarmonyFinalizer]
    [HarmonyPatch(typeof(AOverheat), "Begin")]
    private static void AOverheat_Begin_Finalizer() => _duringOverheat = false;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Ship), "DirectHullDamage")]
    [HarmonyPriority(Priority.First)]
    private static bool Ship_DirectHullDamage_Prefix(Ship __instance, State s, Combat c)
    {
        if (!_duringOverheat || __instance != s.ship) return true;
        if (s.EnumerateAllArtifacts().FirstOrDefault(a => a is MarielleDizzyDuoArtifact) is not { } artifact) return true;
        var artifact2 = (artifact as MarielleDizzyDuoArtifact)!;
        if (artifact2.Charges <= 0) return true;

        artifact2.Charges--;
        _duringOverheat = false;
        artifact.Pulse();
        __instance.NormalDamage(s, c, __instance.overheatDamage, null);
        return false;
    }
}

[HarmonyPatch]
public class MariellePeriDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/Duo/BlazeOfGlory.png")).Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Peri", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Peri", "description"]).Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.MarielleDeck.Deck, Deck.peri]);
    }

    public override List<Tooltip> GetExtraTooltips() =>
    [
        new TTGlossary("status.overdrive", 1),
        new TTGlossary("status.powerdrive", 1),
        new TTGlossary("status.heat", 1)
    ];

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AStatus), "Begin")]
    public static void AStatus_Begin_Prefix(State s, AStatus __instance)
    {
        var artifact = s.EnumerateAllArtifacts().FirstOrDefault(a => a is MariellePeriDuoArtifact);
        // applying positive heat in add mode to enemy
        if (artifact == null
            || __instance.status != Status.heat
            || __instance.statusAmount < 0
            || __instance.mode != AStatusMode.Add
            || __instance.targetPlayer) return;
        __instance.statusAmount += Math.Max(0, s.ship.Get(Status.overdrive)) + Math.Max(0, s.ship.Get(Status.powerdrive));
        artifact.Pulse();
    }
}

[HarmonyPatch]
public class MarielleIsaacDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/Duo/SmokingGun.png")).Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Isaac", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Isaac", "description"]).Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.MarielleDeck.Deck, Deck.goat]);
    }

    public override List<Tooltip> GetExtraTooltips() =>
    [
        new TTGlossary("midrow.drone", ""),
        new TTGlossary("status.heat", 1)
    ];

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(AttackDrone), "GetActions")]
    public static IEnumerable<CodeInstruction> AttackDrone_GetActions_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
    {
        try
        {
            return new SequenceBlockMatcher<CodeInstruction>(instructions)
                .Find(
                    ILMatches.Ldfld<bool>(),
                    ILMatches.Stfld<bool>()
                )
                .Insert(
                    SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.JustInsertion,
                    [
                        new(OpCodes.Ldarg_1),
                        new(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, "ApplyHeat"))
                    ]
                )
                .AllElements();
        }
        catch (Exception e)
        {
            Console.WriteLine("MarielleIsaacDuo's AttackDrone.GetActions patch failed!");
            Console.WriteLine(e);
            return instructions;
        }
    }

    public static AAttack ApplyHeat(AAttack attack, State s)
    {
        var artifact = s.EnumerateAllArtifacts().FirstOrDefault(a => a is MariellePeriDuoArtifact);
        if (artifact != null)
        {
            attack.status = Status.heat;
            attack.statusAmount = 1;
            artifact.Pulse();
        }
        return attack;
    }
}

[HarmonyPatch]
public class MarielleDrakeDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/Duo/DarkPersona.png")).Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Drake", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Drake", "description"]).Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.MarielleDeck.Deck, Deck.eunice]);
    }
    
    public override List<Tooltip> GetExtraTooltips() =>
    [
        new TTGlossary("status.heat", 1),
        StatusMeta.GetTooltips(ModEntry.Instance.Curse.Status, 1)[0]
    ];

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Card), "GetActionsOverridden")]
    public static void Card_GetActionsOverridden_Postfix(State s, Card __instance, ref List<CardAction> __result)
    {
        var artifact = s.EnumerateAllArtifacts().FirstOrDefault(a => a is MarielleDrakeDuoArtifact);
        if (artifact == null) return;
        foreach (var cardAction in __result.SelectMany(ModEntry.Instance.KokoroApi.V2.WrappedActions.GetWrappedCardActionsRecursively))
        {
            if (cardAction is AStatus { mode: AStatusMode.Add, status: Status.heat, statusAmount: < 0 } aStatus)
            {
                aStatus.statusAmount *= -2;
                aStatus.status = ModEntry.Instance.Curse.Status;
            }
        }
    }
}

public class MarielleMaxDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/Duo/OneWithNothing.png")).Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Max", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Max", "description"]).Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.MarielleDeck.Deck, Deck.hacker]);
    }

    public override List<Tooltip> GetExtraTooltips() =>
    [
        new TTGlossary("status.heat", 99),
        StatusMeta.GetTooltips(ModEntry.Instance.Curse.Status, 10)[0]
    ];

    public override void OnTurnEnd(State state, Combat combat)
    {
        // if ya got nothin
        if (state.deck.Count != 0 || combat.hand.Count != 0 || combat.discard.Count != 0) return;
        // roast 'em
        combat.Queue([
            new AStatus
            {
                status = ModEntry.Instance.Curse.Status,
                statusAmount = 10,
                targetPlayer = false,
                timer = 0
            },
            new AStatus
            {
                status = Status.heat,
                statusAmount = 99,
                targetPlayer = false,
                artifactPulse = Key()
            }
        ]);
    }
}

public class MarielleBooksDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/Duo/BlackMagic.png")).Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Books", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["duo", "Books", "description"]).Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.MarielleDeck.Deck, Deck.shard]);

        var costIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/heatAdd.png")).Sprite;
        ModEntry.Instance.KokoroApi.V2.ActionCosts.RegisterResourceCostIcon(new MarielleBooksHeatResource(), costIcon, costIcon);
        ModEntry.Instance.KokoroApi.V2.ActionCosts.RegisterHook(new MarielleBooksActionCostHook(), double.MinValue);
    }
    
    public override List<Tooltip> GetExtraTooltips() =>
    [
        new TTGlossary("status.shard", 1),
        new TTGlossary("status.heat", 1)
    ];

    private class MarielleBooksActionCostHook : IKokoroApi.IV2.IActionCostsApi.IHook
    {
        public bool ModifyActionCost(IKokoroApi.IV2.IActionCostsApi.IHook.IModifyActionCostArgs args)
        {
            if (!args.State.EnumerateAllArtifacts().Any(a => a is MarielleBooksDuoArtifact))
                return false;
            HandleAnyCost(args.Cost);
            return false;

            void HandleAnyCost(IKokoroApi.IV2.IActionCostsApi.ICost cost)
            {
                if (ModEntry.Instance.KokoroApi.V2.ActionCosts.AsResourceCost(cost) is {} resourceCost)
                    HandleResourceCost(resourceCost);
                else if (ModEntry.Instance.KokoroApi.V2.ActionCosts.AsCombinedCost(cost) is {} combinedCost)
                    HandleCombinedCost(combinedCost);
            }
            
            void HandleCombinedCost(IKokoroApi.IV2.IActionCostsApi.ICombinedCost combinedCost)
            {
                foreach (var cost in combinedCost.Costs)
                    HandleAnyCost(cost);
            }

            void HandleResourceCost(IKokoroApi.IV2.IActionCostsApi.IResourceCost cost)
            {
                if (!cost.PotentialResources.Any(r => ModEntry.Instance.KokoroApi.V2.ActionCosts.AsStatusResource(r) is
                        { Status: Status.shard }))
                    return;
                cost.PotentialResources.Add(new MarielleBooksHeatResource());
            }
        }
    }
    
    public class MarielleBooksHeatResource : IKokoroApi.IV2.IActionCostsApi.IResource
    {
        public string ResourceKey => "MarielleBooksHeatResource";
        public int GetCurrentResourceAmount(State state, Combat combat)
        {
            return int.MaxValue;
        }

        public void Pay(State state, Combat combat, int amount)
        {
            combat.QueueImmediate(new AStatus
            {
                status = Status.heat,
                statusAmount = amount,
                targetPlayer = true,
                timer = 0
            });
        }

        public IReadOnlyList<Tooltip> GetTooltips(State state, Combat combat, int amount) =>
            StatusMeta.GetTooltips(Status.heat, 0);
    }
}

public class MarielleCatDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/Duo/Refresh.png")).Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["duo", "CAT", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["duo", "CAT", "description"]).Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.MarielleDeck.Deck, Deck.colorless]);
    }
    
    public override List<Tooltip> GetExtraTooltips() =>
    [
        new TTGlossary("status.serenity", 1)
    ];

    public override void OnTurnEnd(State state, Combat combat)
    {
        if (combat.energy > 0)
        {
            combat.QueueImmediate(new AStatus
            {
                status = Status.serenity,
                statusAmount = 1,
                targetPlayer = true,
                artifactPulse = Key()
            });
        }
    }
}