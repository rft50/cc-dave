using HarmonyLib;
using Jester.Api;
using Jester.External;
using Jester.Generator.Provider;
using Jester.Generator.Strategy;

namespace Jester.Generator;

public class JesterGenerator
{
    public static bool DebugMode = false;
    public static bool Display = false;

    public static readonly List<IProvider> Providers = new()
    {
        new AttackProvider(),
        new EvadeProvider(),
        new ShieldProvider(),
        new InstantMoveProvider(),
        new BayProvider(),
        new MidshiftProvider(),
        new DroneshiftProvider(),
        new StatusProvider(),
        new HealProvider(),
        
        new StatusCostProvider(),
        new AddCardCostProvider(),
        new EqualsZeroCostProvider(),
        new DiscardCardCostProvider(),
        new HurtCostProvider()
    };

    public static readonly List<IStrategy> Strategies = new()
    {
        // inner
        new HalfHalfStrategy(),
        // outer
        new PlainOuterStrategy(),
        new ExhaustStrategy(),
        new CostCardStrategy(),
        new ExhaustCostCardStrategy()
        // full
    };
    
    public static IJesterResult GenerateCard(JesterRequest request)
    {
        IJesterResult data;
        request.Random = new Random(request.Seed);

        if (/*request.Seed % 2 == 0*/true) // change when a Full for all exists
        {
            data = GetStrategiesWeighted(request, StrategyCategory.Outer).Next(request.Random)
                .GenerateCard(request, Providers, 5);
        }
        else
        {
            data = GetStrategiesWeighted(request, StrategyCategory.Full).Next(request.Random)
                .GenerateCard(request, Providers, 5);
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

    public static IJesterResult CallInnerStrategy(IJesterRequest request, IList<IProvider> providers, int maxActions)
    {
        return GetStrategiesWeighted(request, StrategyCategory.Inner).Next(request.Random).GenerateCard(request, providers, maxActions);
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