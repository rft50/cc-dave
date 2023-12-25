using HarmonyLib;
using Jester.External;
using Jester.Generator.Provider;
using Jester.Generator.Strategy;

namespace Jester.Generator;

public class JesterGenerator
{
    public static bool DebugMode = false;
    public static bool Display = false;

    private static readonly List<IProvider> Providers = new()
    {
        new AttackProvider(),
        new EvadeProvider(),
        new ShieldProvider(),
        new InstantMoveProvider(),
        new BayProvider(),
        new MidshiftProvider(),
        new DroneshiftProvider(),
        new StatusProvider(),
        
        new StatusCostProvider(),
        new AddCardCostProvider(),
        new EqualsZeroCostProvider(),
        new DiscardCardCostProvider()
    };

    private static readonly List<IStrategy> Strategies = new()
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
    
    public static JesterResult GenerateCard(JesterRequest request)
    {
        JesterResult data;
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
            data.CardData.flippable = true;
        
        JesterCLI.Main();

        return data;
    }

    public static JesterResult CallInnerStrategy(JesterRequest request, List<IProvider> providers, int maxActions)
    {
        return GetStrategiesWeighted(request, StrategyCategory.Inner).Next(request.Random).GenerateCard(request, providers, maxActions);
    }

    private static WeightedRandom<IStrategy> GetStrategiesWeighted(JesterRequest request, StrategyCategory? category = null)
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

        public HashSet<string> Tags { get; } = new();
        public IProvider Provider { get; } = new AttackProvider();
        public int GetActionCount() => Actions.Count;

        public List<CardAction> GetActions(State s, Combat c) => Actions;

        public int GetCost() => 0;

        public IEntry? GetUpgradeA(JesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public IEntry? GetUpgradeB(JesterRequest request, out int cost)
        {
            cost = 0;
            return null;
        }

        public void AfterSelection(JesterRequest request)
        {
        }
    }
}