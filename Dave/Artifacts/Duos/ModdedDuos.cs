using Dave.Api;
using Dave.External;
using Nanoray.PluginManager;
using Nickel;

namespace Dave.Artifacts.Duos;

public class DaveDynaDuoArtifact : Artifact, IDuoArtifact
{
    private static ISpriteEntry _red = null!;
    private static ISpriteEntry _black = null!;
    private static ISpriteEntry _off = null!;
    public bool IsRed = true;
    public bool Ready = true;
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.ModRegistry.AwaitApi<IDynaApi>("Shockah.Dyna", dyna =>
        {
            _red = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Duo/DaveDynaDuo_red.png"));
            _black = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Duo/DaveDynaDuo_black.png"));
            _off = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Duo/DaveDynaDuo_off.png"));
            
            helper.Content.Artifacts.RegisterArtifact("DaveDynaDuoArtifact", new()
            {
                ArtifactType = typeof(DaveDynaDuoArtifact),
                Meta = new()
                {
                    owner = duoApi.DuoArtifactVanillaDeck
                },
                Sprite = _red.Sprite,
                Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Dyna", "name"]).Localize,
                Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Dyna", "description"])
                    .Localize
            });
            duoApi.RegisterDuoArtifact(typeof(DaveDynaDuoArtifact), [ModEntry.Instance.DaveDeck.Deck, dyna.DynaDeck.Deck]);
            
            dyna.RegisterHook(new DaveDynaHook(), 0);
        });
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        Ready = true;
    }

    public override Spr GetSprite() => !Ready ? _off.Sprite : IsRed ? _red.Sprite : _black.Sprite;

    private class DaveDynaHook : IDynaHook
    {
        public void OnChargeFired(State state, Combat combat, Ship targetShip, int worldX)
        {
            if (targetShip != combat.otherShip) return;
            var artifact = state.artifacts.OfType<DaveDynaDuoArtifact>().FirstOrDefault();
            if (artifact is not { Ready: true }) return;
            artifact.Ready = false;
            combat.QueueImmediate(new AStatus
            {
                status = artifact.IsRed ? ModEntry.Instance.RedRigging.Status : ModEntry.Instance.BlackRigging.Status,
                targetPlayer = true,
                statusAmount = 1,
                mode = AStatusMode.Add,
                artifactPulse = artifact.Key()
            });
            artifact.IsRed = !artifact.IsRed;
        }
    }
}

public class DaveDraculaDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.ModRegistry.AwaitApi<IDraculaApi>("Shockah.Dracula", drac =>
        {
            helper.Content.Artifacts.RegisterArtifact("DaveDraculaDuoArtifact", new()
            {
                ArtifactType = typeof(DaveDraculaDuoArtifact),
                Meta = new()
                {
                    owner = duoApi.DuoArtifactVanillaDeck
                },
                Sprite = helper.Content.Sprites
                    .RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Duo/DaveDraculaDuo.png"))
                    .Sprite,
                Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Dracula", "name"]).Localize,
                Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Dracula", "description"])
                    .Localize
            });
            duoApi.RegisterDuoArtifact(typeof(DaveDraculaDuoArtifact), [ModEntry.Instance.DaveDeck.Deck, drac.DraculaDeck.Deck]);
            
            ModEntry.Instance.RollHookManager.Register(new DaveDraculaRollHook(), 0);
        });
    }

    private class DaveDraculaRollHook : IDaveApi.IRollHook
    {
        public void OnRoll(State state, Combat combat, bool isRed, bool isBlack, bool isRoll)
        {
            var artifact = state.EnumerateAllArtifacts().FirstOrDefault(a => a is DaveDraculaDuoArtifact);
            if (artifact == null || !isRoll || !isRed) return;
            combat.Queue(new AStatus
            {
                status = ModEntry.Instance.DraculaApi!.BleedingStatus.Status,
                statusAmount = 1,
                targetPlayer = false,
                artifactPulse = artifact.Key()
            });
        }
    }
}

public class DaveEddieDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.ModRegistry.AwaitApi<IEddieApi>("TheJazMaster.Eddie", eddie =>
        {
            helper.Content.Artifacts.RegisterArtifact("DaveEddieDuoArtifact", new()
            {
                ArtifactType = typeof(DaveEddieDuoArtifact),
                Meta = new()
                {
                    owner = duoApi.DuoArtifactVanillaDeck
                },
                Sprite = helper.Content.Sprites
                    .RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Duo/DaveEddieDuo.png"))
                    .Sprite,
                Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Eddie", "name"]).Localize,
                Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Eddie", "description"])
                    .Localize
            });
            duoApi.RegisterDuoArtifact(typeof(DaveEddieDuoArtifact), [ModEntry.Instance.DaveDeck.Deck, eddie.EddieDeck]);
        });
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (state.ship.Get(ModEntry.Instance.RedRigging.Status) < 1) return;
        combat.Queue([new AStatus
        {
            status = ModEntry.Instance.RedRigging.Status,
            statusAmount = -1,
            targetPlayer = true,
            timer = 0f,
            artifactPulse = Key()
        },
        new AEnergy
        {
            changeAmount = 1
        }]);
    }
}

public class DaveJohnsonDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.ModRegistry.AwaitApi<IJohnsonApi>("Shockah.Johnson", johnson =>
        {
            helper.Content.Artifacts.RegisterArtifact("DaveJohnsonDuoArtifact", new()
            {
                ArtifactType = typeof(DaveJohnsonDuoArtifact),
                Meta = new()
                {
                    owner = duoApi.DuoArtifactVanillaDeck
                },
                Sprite = helper.Content.Sprites
                    .RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Duo/DaveJohnsonDuo.png"))
                    .Sprite,
                Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Johnson", "name"]).Localize,
                Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Johnson", "description"])
                    .Localize
            });
            duoApi.RegisterDuoArtifact(typeof(DaveJohnsonDuoArtifact), [ModEntry.Instance.DaveDeck.Deck, johnson.JohnsonDeck.Deck]);
        });
    }

    public override void OnTurnEnd(State state, Combat combat)
    {
        if (state.ship.Get(Status.overdrive) >= 0) return;
        combat.Queue(new AStatus
        {
            status = Status.temporaryCheap,
            statusAmount = 1,
            targetPlayer = true,
            artifactPulse = Key()
        });
    }
}

public class DaveSogginsDuoArtifact : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.ModRegistry.AwaitApi<ISogginsApi>("Shockah.Soggins", soggins =>
        {
            helper.Content.Artifacts.RegisterArtifact("DaveSogginsDuoArtifact", new()
            {
                ArtifactType = typeof(DaveSogginsDuoArtifact),
                Meta = new()
                {
                    owner = duoApi.DuoArtifactVanillaDeck
                },
                Sprite = helper.Content.Sprites
                    .RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Artifact/Duo/DaveSogginsDuo.png"))
                    .Sprite,
                Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Soggins", "name"]).Localize,
                Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Soggins", "description"])
                    .Localize
            });
            duoApi.RegisterDuoArtifact(typeof(DaveSogginsDuoArtifact), [ModEntry.Instance.DaveDeck.Deck, soggins.SogginsVanillaDeck]);
            
            soggins.RegisterSmugHook(new DaveSogginsSmugHook(), -100);
        });
    }

    private class DaveSogginsSmugHook : ISmugHook
    {
        public double ModifySmugBotchChance(State state, Ship ship, Card? card, double chance)
        {
            if (state.ship != ship || !state.EnumerateAllArtifacts().Any(a => a is DaveSogginsDuoArtifact))
                return chance;
            if (state.ship.Get(ModEntry.Instance.RedRigging.Status) < 1 || state.ship.Get(ModEntry.Instance.BlackRigging.Status) < 1)
                return chance;
            return chance * 3;
        }
        
        public double ModifySmugDoubleChance(State state, Ship ship, Card? card, double chance)
        {
            if (state.ship != ship || !state.EnumerateAllArtifacts().Any(a => a is DaveSogginsDuoArtifact))
                return chance;
            if (state.ship.Get(ModEntry.Instance.RedRigging.Status) < 1 || state.ship.Get(ModEntry.Instance.BlackRigging.Status) < 1)
                return chance;
            return chance * 3;
        }
    }
}