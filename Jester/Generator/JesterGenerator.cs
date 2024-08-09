using HarmonyLib;
using Jester.External;
using Jester.Generator.Provider.Books;
using Jester.Generator.Provider.Common;
using Jester.Generator.Provider.Drake;
using Jester.Generator.Provider.Isaac;
using Jester.Generator.Strategy.Books;
using Jester.Generator.Strategy.Common;
using IStrategy = Jester.Api.IJesterApi.IStrategy;
using IJesterRequest = Jester.Api.IJesterApi.IJesterRequest;
using IJesterResult = Jester.Api.IJesterApi.IJesterResult;
using IEntry = Jester.Api.IJesterApi.IEntry;
using IProvider = Jester.Api.IJesterApi.IProvider;
using StrategyCategory = Jester.Api.IJesterApi.StrategyCategory;

namespace Jester.Generator;

public class JesterGenerator
{
    public static bool DebugMode = false;
    public static bool Display = false;

    public static readonly List<IProvider> Providers = new()
    {
        // Common Providers
        new AttackProvider(),
        new EvadeProvider(),
        new ShieldProvider(),
        new InstantMoveProvider(),
        new DroneProvider(),
        new MissileProvider(),
        new MineProvider(),
        new DroneshiftProvider(),
        new StatusProvider(),
        new HealProvider(),
        new DrawProvider(),
        new OverdriveProvider(),

        // Common Costs
        new StatusCostProvider(),
        new AddCardCostProvider(),
        new EqualsZeroCostProvider(),
        new DiscardCardCostProvider(),
        new HurtCostProvider(),
        
        // Isaac
        new MidshiftProvider(),

        // Drake
        new LessHeatProvider(),
        new HeatCostProvider(),
        new SerenityProvider(),

        // Books
        new ShardProvider(),
        new MaxShardProvider()
    };

    public static readonly List<IStrategy> Strategies = new()
    {
        // inner
        new HalfHalfStrategy(),
        // outer
        new PlainOuterStrategy(),
        new ExhaustStrategy(),
        new CostCardStrategy(),
        new ExhaustCostCardStrategy(),
        // full
        new BooksStrategy()
    };

    public static IJesterResult GenerateCard(IJesterRequest request)
    {
        ModManifest.Helper.ModData.TryGetModData<ProfileSettings>(request.State, "Settings", out var settings);
        IJesterResult data;
        request.Random = new Rand((uint)request.Seed);
        request.ActionLimit = 3 + request.Random.NextInt() % ((settings?.ActionCap ?? 5) - 2);

        {
            var fullStrategies = GetStrategiesWeighted(request, StrategyCategory.Full);
            if (request.Seed % 3 == 0 && fullStrategies.Items.Count > 0)
            {
                data = fullStrategies.Next(request.Random)
                    .GenerateCard(request, Providers);
            }
            else
            {
                data = GetStrategiesWeighted(request, StrategyCategory.Outer).Next(request.Random)
                    .GenerateCard(request, Providers);
            }
        }

        if (Display)
        {
            var text = new TTText
            {
                text = data.Entries.Select(e => $"{e.GetType().Name}: {e.GetCost()} pts")
                    .Join(delimiter: "\n")
            };
            text.text = $"Seed: {request.Seed}\n" + text.text;

            data.Entries.Insert(0, new ArbitraryEntry
            {
                Actions = new List<CardAction>
                {
                    new TooltipAction
                    {
                        Tooltips = new List<Tooltip>
                        {
                            text
                        }
                    }
                }
            });
            data.Entries.Add(new ArbitraryEntry
            {
                Actions = new List<CardAction>
                {
                    new ADummyAction()
                }
            });
        }

        if (DebugMode)
        {
            var tempData = request.CardData;
            tempData.flippable = true;
            request.CardData = tempData;
        }

        // JesterCLI.Main();

        return data;
    }

    public static IJesterResult UpgradeResultA(IJesterRequest origReq, IJesterResult origRes)
    {
        // things return true if they have a chance to be re-applied
        var options = new List<Func<IJesterRequest, IJesterResult, bool>>
        {
            ApplyUpgrade(Upgrade.A),
            UpgradeOptions.Flippable,
            UpgradeOptions.RemoveExhaust,
            UpgradeOptions.RemoveCost,
            UpgradeOptions.ReduceEnergy,
            UpgradeOptions.AddAction
        };

        return ApplyUpgradeOptions(origReq, origRes, options);
    }

    public static IJesterResult UpgradeResultB(IJesterRequest origReq, IJesterResult origRes)
    {
        var tryBox = new[] { 6 + origReq.Seed % 4 }; // how many tries are left to apply a cost

        // things return true if they have a chance to be re-applied
        var options = new List<Func<IJesterRequest, IJesterResult, bool>>
        {
            ApplyUpgrade(Upgrade.B),
            UpgradeOptions.Flippable,
            UpgradeOptions.AddCost(tryBox),
            UpgradeOptions.IncreaseEnergy(tryBox)
        };

        return ApplyUpgradeOptions(origReq, origRes, options);
    }

    private static Func<IJesterRequest, IJesterResult, bool> ApplyUpgrade(Upgrade upDir)
    {
        return (request, result) =>
        {
            var pts = result.SparePoints;
            var origPts = pts;
            var ups = ModManifest.JesterApi.PerformUpgrade(request, ref pts, upDir, 1);
            result.SparePoints -= origPts - pts;
            return ups > 0;
        };
    }

    private static IJesterResult ApplyUpgradeOptions(IJesterRequest origReq, IJesterResult origRes,
        List<Func<IJesterRequest, IJesterResult, bool>> options)
    {
        const int upgradeBonus = 20;

        var req = new JesterRequest
        {
            Seed = origReq.Seed,
            FirstAction = origReq.FirstAction,
            State = origReq.State,
            BasePoints = origReq.BasePoints += upgradeBonus,
            CardData = origReq.CardData,
            ActionLimit = origReq.ActionLimit,
            SingleUse = origReq.SingleUse,
            Random = origReq.Random.Offshoot(),
            Whitelist = origReq.Whitelist,
            Blacklist = origReq.Blacklist,
            CardMeta = origReq.CardMeta
        };

        var res = new JesterResult
        {
            CardData = origRes.CardData,
            Entries = origRes.Entries.ToList(),
            SparePoints = origRes.SparePoints
        };

        res.SparePoints += upgradeBonus;
        req.Entries = res.Entries;

        while (options.Count > 0)
        {
            var option = ModManifest.JesterApi.GetJesterUtil().GetRandom(options, req.Random);
            options.Remove(option);
            if (option(req, res))
            {
                options.Add(option);
            }
        }

        return res;
    }

    public static IJesterResult CallInnerStrategy(IJesterRequest request, IEnumerable<IProvider> providers)
    {
        return GetStrategiesWeighted(request, StrategyCategory.Inner).Next(request.Random)
            .GenerateCard(request, providers);
    }

    private static WeightedRandom<IStrategy> GetStrategiesWeighted(IJesterRequest request,
        StrategyCategory? category = null)
    {
        return new WeightedRandom<IStrategy>(Strategies
            .Where(s => category == null || s.GetStrategyCategory() == category)
            .Select(s => new WeightedItem<IStrategy>(s.GetWeight(request), s))
            .Where(s => s.Weight > 0)
        );
    }

    private class TooltipAction : CardAction
    {
        public List<Tooltip> Tooltips = null!;

        public override List<Tooltip> GetTooltips(State s) => Tooltips;
    }

    private class ArbitraryEntry : IEntry
    {
        public List<CardAction> Actions = null!;

        public IReadOnlySet<string> Tags { get; } = new HashSet<string>();

        public IEnumerable<CardAction> GetActions(State s, Combat c) => Actions;

        public int GetCost() => 0;

        public IEnumerable<(double, IEntry)> GetUpgradeOptions(IJesterRequest request, Upgrade upDir)
        {
            return new List<(double, IEntry)>();
        }

        public void AfterSelection(IJesterRequest request)
        {
        }
    }

    private static class UpgradeOptions
    {

        public static bool Flippable(IJesterRequest request, IJesterResult result)
        {
            if (result.SparePoints < 10) return false;
            if (!result.Entries.Any(e => e.Tags.Contains("flippable"))) return false;
            result.SparePoints -= 10;
            var data = result.CardData;
            data.flippable = true;
            result.CardData = data;
            request.CardData = data;
            return false;
        }

        public static bool RemoveExhaust(IJesterRequest request, IJesterResult result)
        {
            if (!result.CardData.exhaust) return false;
            if (result.SparePoints < 25) return false;
            if (result.Entries.Any(e => e.Tags.Contains("mustExhaust"))) return false;
            result.SparePoints -= 25;
            var data = result.CardData;
            data.exhaust = false;
            result.CardData = data;
            request.CardData = data;
            return false;
        }

        public static bool RemoveCost(IJesterRequest request, IJesterResult result)
        {
            var cost = result.Entries.FirstOrDefault(e => e.Tags.Contains("cost"));
            if (cost == null) return false;
            if (-cost.GetCost() > result.SparePoints) return false;
            result.SparePoints += cost.GetCost();
            result.Entries.Remove(cost);
            return false;
        }

        public static bool ReduceEnergy(IJesterRequest request, IJesterResult result)
        {
            if (result.CardData.cost == 0) return false;
            var cost = Math.Max(result.CardData.cost * 8, 16);
            if (result.SparePoints < cost) return false;
            result.SparePoints -= cost;
            var data = result.CardData;
            data.cost -= 1;
            result.CardData = data;
            request.CardData = data;
            return false;
        }

        public static bool AddAction(IJesterRequest request, IJesterResult result)
        {
            var actionCount = result.Entries.Sum(e => e.GetActions(DB.fakeState, DB.fakeCombat).Count());
            if (request.Seed % request.ActionLimit > actionCount) return false;
            request.MinCost = 1;
            request.MaxCost = result.SparePoints;
            var option = ModManifest.JesterApi.GetRandomEntry(request, Providers, request.ActionLimit - actionCount);

            if (option == null) return false;
            
            result.Entries.Add(option);
            result.SparePoints -= option.GetCost();
            return false;
        }

        public static Func<IJesterRequest, IJesterResult, bool> AddCost(int[] tryBox)
        {
            return (request, result) =>
            {
                if (tryBox[0] <= 0) return false;
                tryBox[0]--;
                var stats = UpgradePotentialStatistics(request, result);
                if (stats.Count(e => e <= 0) >= stats.Count/2) return true;
                
                var actionCount = result.Entries.Sum(e => e.GetActions(DB.fakeState, DB.fakeCombat).Count());
                if (actionCount >= request.ActionLimit) return false;
                var whitelist = request.Whitelist;
                request.Whitelist = new HashSet<string>
                {
                    "cost"
                };
                request.MinCost = -request.BasePoints * 2 / 3;
                request.MaxCost = -stats.Min();
                var option = ModManifest.JesterApi.GetRandomEntry(request, Providers, request.ActionLimit - actionCount);
                request.Whitelist = whitelist;

                if (option == null) return true;
                
                option.AfterSelection(request);
                result.SparePoints -= option.GetCost();
                result.Entries.Add(option);

                tryBox[0] = 0;
                return false;
            };
        }

        public static Func<IJesterRequest, IJesterResult, bool> IncreaseEnergy(int[] tryBox)
        {
            return (request, result) =>
            {
                if (result.CardData.cost == 0) return false;
                if (tryBox[0] <= 0) return false;
                tryBox[0]--;
                var stats = UpgradePotentialStatistics(request, result);
                if (stats.Count(e => e <= 0) >= stats.Count) return true;
                
                if (!stats.Any(e => e is > 0 and <= 20))
                    return true;

                result.SparePoints += 20;
                var data = result.CardData;
                data.cost += 1;
                result.CardData = data;
                request.CardData = data;

                tryBox[0] = 0;
                return false;
            };
        }

        private static List<int> UpgradePotentialStatistics(IJesterRequest request, IJesterResult result)
        {
            var sparePoints = result.SparePoints;
            var data = result.Entries.Select(e =>
                    e.GetUpgradeOptions(request, Upgrade.B)
                        .Select(u => u.Item2.GetCost()))
                .Where(e => e.Any())
                .Select(e => e.Max())
                .Select(e => e - sparePoints)
                .ToList();
            return data;
        }
    }
}