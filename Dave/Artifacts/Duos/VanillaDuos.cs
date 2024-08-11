using System.Reflection;
using System.Reflection.Emit;
using Dave.Actions;
using Dave.Api;
using Dave.External;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Nickel;

namespace Dave.Artifacts.Duos;

public interface IDuoArtifact
{
    public static abstract void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi);
}

[HarmonyPatch]
public class DaveDizzyDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact("DaveDizzyDuoArtifact", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Duo/DaveDizzyDuo.png"))
                .Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Dizzy", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Dizzy", "description"])
                .Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.DaveDeck.Deck, Deck.dizzy]);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AHurt), "Begin")]
    public static void AHurt_Begin_Postfix(State s, Combat c, AHurt __instance)
    {
        if (!__instance.targetPlayer) return;
        var artifact = s.EnumerateAllArtifacts().FirstOrDefault(a => a is DaveDizzyDuoArtifact);
        if (artifact == null) return;
        if (!ModEntry.Instance.Helper.ModData.ObtainModData(__instance, "DaveDizzyDuoProc", () => false)) return;
        c.QueueImmediate(new AStatus
        {
            targetPlayer = true,
            statusAmount = 1,
            status = Status.tempShield,
            artifactPulse = artifact.Key()
        });
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Combat), "TryPlayCard")]
    public static IEnumerable<CodeInstruction> Combat_TryPlayCard_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        try
        {
            return new SequenceBlockMatcher<CodeInstruction>(instructions)
                .Find(
                    ILMatches.Call("GetActionsOverridden")
                )
                .Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
                [
                    new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(DaveDizzyDuoArtifact), nameof(ProcessActions)))
                ])
                .AllElements();
        }
        catch (Exception e)
        {
            ModEntry.Instance.Logger.Log(LogLevel.Error, e, "Combat_TryPlayCard_Transpiler failed");
            throw;
        }
    }

    public static List<CardAction> ProcessActions(List<CardAction> actions)
    {
        return actions.SelectMany(ModEntry.Instance.KokoroApi.Actions.GetWrappedCardActionsRecursively)
            .Select(a =>
            {
                if (a is AHurt)
                {
                    ModEntry.Instance.Helper.ModData.SetModData(a, "DaveDizzyDuoProc", true);
                }

                return a;
            }).ToList();
    }
}

[HarmonyPatch]
public class DaveRiggsDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact("DaveRiggsDuoArtifact", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Duo/DaveRiggsDuo.png"))
                .Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Riggs", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Riggs", "description"])
                .Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.DaveDeck.Deck, Deck.riggs]);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AMove), "Begin")]
    public static void AMove_Begin_Prefix(State s, AMove __instance)
    {
        var artifact = s.EnumerateAllArtifacts().FirstOrDefault(a => a is DaveRiggsDuoArtifact);
        if (artifact == null || !__instance.isRandom) return;
        __instance.dir += Math.Sign(__instance.dir);
    }
}

public class DavePeriDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact("DavePeriDuoArtifact", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Duo/DavePeriDuo.png"))
                .Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Peri", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Peri", "description"])
                .Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.DaveDeck.Deck, Deck.peri]);
    }

    public override int ModifyBaseDamage(int baseDamage, Card? card, State state, Combat? combat, bool fromPlayer)
    {
        var meta = card?.GetMeta();
        if (state.ship.Get(Status.overdrive) < 0 && meta != null && (meta.deck == Deck.peri || meta.deck == ModEntry.Instance.DaveDeck.Deck))
        {
            return 1;
        }

        return 0;
    }
}

public class DaveIsaacDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact("DaveIsaacDuoArtifact", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Duo/DaveIsaacDuo.png"))
                .Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Isaac", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Isaac", "description"])
                .Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.DaveDeck.Deck, Deck.goat]);
    }

    public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition,
        int handCount)
    {
        if (!card.GetActions(state, combat).Any(a =>
                ModEntry.Instance.KokoroApi.Actions.GetWrappedCardActionsRecursively(a).Any(b => b is ASpawn))) return;
        
        var data = RandomChoiceActionFactory.RandomRoll(state, combat);

        if (data.IsBlack)
        {
            combat.QueueImmediate(new AStatus
            {
                status = Status.bubbleJuice,
                statusAmount = 1,
                targetPlayer = true,
                artifactPulse = Key()
            });
        }
    }
}

[HarmonyPatch]
public class DaveDrakeDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact("DaveDrakeDuoArtifact", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Duo/DaveDrakeDuo.png"))
                .Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Drake", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Drake", "description"])
                .Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.DaveDeck.Deck, Deck.eunice]);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AStatus), "Begin")]
    public static void AStatus_Begin_Prefix(State s, out int __state)
    {
        __state = s.ship.Get(Status.heat);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AStatus), "Begin")]
    public static void AStatus_Begin_Postfix(State s, Combat c, int __state)
    {
        var latest = s.ship.Get(Status.heat);

        var artifact = s.EnumerateAllArtifacts().FirstOrDefault(a => a is DaveDrakeDuoArtifact);
        if (artifact == null) return;
        
        if (__state < 3 && latest >= 3)
        {
            c.Queue([
                new AStatus
                {
                    status = ModEntry.Instance.RedRigging.Status,
                    statusAmount = 1,
                    targetPlayer = true,
                    artifactPulse = artifact.Key(),
                    timer = 0
                },
                new AStatus
                {
                    status = ModEntry.Instance.BlackRigging.Status,
                    statusAmount = 1,
                    targetPlayer = true
                }
            ]);
        }
    }
}

public class DaveMaxDuoArtifact : Artifact, IDuoArtifact
{
    private static ISpriteEntry _readySprite = null!;
    private static ISpriteEntry _notReadySprite = null!;
    public bool Ready = true;
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        _readySprite = helper.Content.Sprites
            .RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Duo/DaveMaxDuo.png"));
        _notReadySprite = helper.Content.Sprites
            .RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Duo/DaveMaxDuo_off.png"));
        
        helper.Content.Artifacts.RegisterArtifact("DaveMaxDuoArtifact", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck
            },
            Sprite = _readySprite.Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Max", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Max", "description"])
                .Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.DaveDeck.Deck, Deck.hacker]);
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        Ready = true;
    }

    public override void OnTurnEnd(State state, Combat combat)
    {
        if (!Ready
            || (state.ship.Get(ModEntry.Instance.RedRigging.Status) <= 0
                && state.ship.Get(ModEntry.Instance.BlackRigging.Status) <= 0)) return;
        Ready = false;
        combat.Queue([new AStatus
        {
            status = ModEntry.Instance.RedRigging.Status,
            statusAmount = 1,
            targetPlayer = true,
            timer = 0f,
            artifactPulse = Key()
        }, new AStatus
        {
            status = ModEntry.Instance.BlackRigging.Status,
            statusAmount = 1,
            targetPlayer = true
        }]);
    }

    public override Spr GetSprite() => Ready ? _readySprite.Sprite : _notReadySprite.Sprite;
}

public class DaveBooksDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact("DaveBooksDuoArtifact", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Duo/DaveBooksDuo.png"))
                .Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Books", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Books", "description"])
                .Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.DaveDeck.Deck, Deck.shard]);
        
        ModEntry.Instance.RollModifierManager.Register(new DaveBooksRollModifier(), -10);
    }

    private class DaveBooksRollModifier : IDaveApi.IRollModifier
    {
        public (bool, bool)? ModifyRoll(State state, Combat combat)
        {
            if (state.ship.Get(Status.shard) < 2) return null;
            var artifact = state.EnumerateAllArtifacts().FirstOrDefault(a => a is DaveBooksDuoArtifact);
            if (artifact == null) return null;
            combat.QueueImmediate(new AStatus { status = Status.shard, targetPlayer = true, statusAmount = -2, mode = AStatusMode.Add, timer = 0, artifactPulse = artifact.Key() });
            return (true, true);
        }
    }
}

[HarmonyPatch]
public class DaveCatDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact("DaveCATDuoArtifact", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Duo/DaveCATDuo.png"))
                .Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "CAT", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "CAT", "description"])
                .Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.DaveDeck.Deck, Deck.colorless]);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ACardOffering), "BeginWithRoute")]
    public static void ACardOffering_BeginWithRoute_Prefix(State s, ACardOffering __instance)
    {
        if (!s.EnumerateAllArtifacts().Any(a => a is DaveCatDuoArtifact)) return;
        // this is intentionally a ceiling halve
        __instance.amount -= __instance.amount / 2;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ACardOffering), "BeginWithRoute")]
    public static void ACardOffering_BeginWithRoute_Postfix(State s, ref Route __result)
    {
        if (!s.EnumerateAllArtifacts().Any(a => a is DaveCatDuoArtifact)) return;
        if (__result is not CardReward reward) return;
        ModEntry.Instance.Helper.ModData.SetModData(__result, "DaveCATDuoProc", true);
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CardReward), "TakeCard")]
    public static void CardReward_TakeCard_Postfix(G g, Card card, CardReward __instance)
    {
        if (!ModEntry.Instance.Helper.ModData.ObtainModData(__instance, "DaveCATDuoProc", () => false)) return;
        ModEntry.Instance.Helper.ModData.SetModData(__instance, "DaveCATDuoProc", false);
        __instance.TakeCard(g, card.CopyWithNewId());
    }
}