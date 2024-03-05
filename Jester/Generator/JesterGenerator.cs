using HarmonyLib;
using Jester.Api;
using Jester.External;
using Jester.Generator.Provider;
using Jester.Generator.Provider.Books;
using Jester.Generator.Provider.Common;
using Jester.Generator.Provider.Drake;
using Jester.Generator.Strategy;
using Jester.Generator.Strategy.Books;
using Jester.Generator.Strategy.Common;

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
        new BayProvider(),
        new MidshiftProvider(),
        new DroneshiftProvider(),
        new StatusProvider(),
        new HealProvider(),
        new DrawProvider(),
        
        // Common Costs
        new StatusCostProvider(),
        new AddCardCostProvider(),
        new EqualsZeroCostProvider(),
        new DiscardCardCostProvider(),
        new HurtCostProvider(),
        
        // Drake
        new LessHeatProvider(),
        new HeatCostProvider(),
        new SerenityProvider(),
        
        // Books
        new ShardProvider()
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

    public static int ActionCap = 5;
    
    public static IJesterResult GenerateCard(IJesterRequest request)
    {
        IJesterResult data;
        request.Random = new Rand((uint) request.Seed);
        request.ActionLimit = 3 + request.Random.NextInt() % (ActionCap - 3);

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
        
        JesterCLI.Main();

        return data;
    }

    public static IJesterResult UpgradeResultA(IJesterRequest origReq, IJesterResult origRes)
    {
        // things return true if they have a chance to be re-applied
        var options = new List<Func<IJesterRequest, IJesterResult, bool>>
        {
            UpgradeOptions.UpgradeA,
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
        var boolBox = new[] { origReq.Seed % 12 == 5 }; // has a cost been taken yet
        
        // things return true if they have a chance to be re-applied
        var options = new List<Func<IJesterRequest, IJesterResult, bool>>
        {
            UpgradeOptions.UpgradeB,
            UpgradeOptions.Flippable,
            UpgradeOptions.AddCost(boolBox),
            UpgradeOptions.IncreaseEnergy(boolBox)
        };

        return ApplyUpgradeOptions(origReq, origRes, options);
    }

    private static IJesterResult ApplyUpgradeOptions(IJesterRequest origReq, IJesterResult origRes, List<Func<IJesterRequest, IJesterResult, bool>> options)
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
            Blacklist = origReq.Blacklist
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

    public static IJesterResult CallInnerStrategy(IJesterRequest request, IList<IProvider> providers)
    {
        return GetStrategiesWeighted(request, StrategyCategory.Inner).Next(request.Random).GenerateCard(request, providers);
    }

    private static WeightedRandom<IStrategy> GetStrategiesWeighted(IJesterRequest request, StrategyCategory? category = null)
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

        public ISet<string> Tags { get; } = new HashSet<string>();
        public IProvider Provider { get; } = new AttackProvider();
        public int GetActionCount() => Actions.Count;

        public IList<CardAction> GetActions(State s, Combat c) => Actions;

        public int GetCost() => 0;

        public IEntry? GetUpgradeA(IJesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public IEntry? GetUpgradeB(IJesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public void AfterSelection(IJesterRequest request)
        {
        }
    }
}

internal static class UpgradeOptions
{

    public static bool UpgradeA(IJesterRequest request, IJesterResult result)
    {
        var pts = result.SparePoints;
        var origPts = pts;
        var ups = ModManifest.JesterApi.PerformUpgradeA(request, result.Entries, ref pts, 1);
        result.SparePoints -= origPts - pts;
        return ups > 0;
    }

    public static bool UpgradeB(IJesterRequest request, IJesterResult result)
    {
        var pts = result.SparePoints;
        var origPts = pts;
        var ups = ModManifest.JesterApi.PerformUpgradeB(request, result.Entries, ref pts, 1);
        result.SparePoints -= origPts - pts;
        return ups > 0;
    }

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
        if (result.SparePoints < 10) return false;
        if (result.Entries.Any(e => e.Tags.Contains("mustExhaust"))) return false;
        result.SparePoints -= 10;
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
        var cost = Math.Max(result.CardData.cost * 5, 10);
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
        var actionCount = result.Entries.Sum(e => e.GetActionCount());
        if (request.Seed % JesterGenerator.ActionCap > actionCount) return false;
        request.MinCost = 1;
        request.MaxCost = result.SparePoints;
        var options = ModManifest.JesterApi.GetOptionsFromProvidersWeighted(request, JesterGenerator.Providers)
            .Where(e => actionCount + e.GetActionCount() <= JesterGenerator.ActionCap)
            .ToList();
            
        if (options.Count == 0) return false;
        var entry = ModManifest.JesterApi.GetJesterUtil().GetRandom(options, request.Random);
        result.Entries.Add(entry);
        result.SparePoints -= entry.GetCost();
        return false;
    }

    public static Func<IJesterRequest, IJesterResult, bool> AddCost(bool[] boolBox)
    {
        return (request, result) =>
        {
            if (boolBox[0]) return false;
            var actionCount = result.Entries.Sum(e => e.GetActionCount());
            if (actionCount >= JesterGenerator.ActionCap) return false;
            var whitelist = request.Whitelist;
            request.Whitelist = new HashSet<string>
            {
                "cost"
            };
            request.MinCost = -request.BasePoints;
            request.MaxCost = request.MinCost / 2;
            var options = ModManifest.JesterApi.GetOptionsFromProvidersWeighted(request, JesterGenerator.Providers)
                .Where(e => actionCount + e.GetActionCount() <= JesterGenerator.ActionCap)
                .ToList();
            request.Whitelist = whitelist;
            
            if (options.Count == 0) return false;
            var cost = ModManifest.JesterApi.GetJesterUtil().GetRandom(options, request.Random);
            cost.AfterSelection(request);
            result.SparePoints -= cost.GetCost();
            result.Entries.Add(cost);

            boolBox[0] = true;
            return false;
        };
    }

    public static Func<IJesterRequest, IJesterResult, bool> IncreaseEnergy(bool[] boolBox)
    {
        return (request, result) =>
        {
            if (boolBox[0]) return false;
            
            if (result.CardData.cost == 0) return false;
            result.SparePoints += 15;
            var data = result.CardData;
            data.cost += 1;
            result.CardData = data;
            request.CardData = data;

            boolBox[0] = true;
            return false;
        };
    }
}