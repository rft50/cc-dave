using HarmonyLib;
using Jester.Generator.Provider;
using Jester.Generator.Strategy;

namespace Jester.Generator;

public class JesterGenerator
{
    public static bool DebugMode = true;
    
    public static JesterResult GenerateCard(JesterRequest request)
    {
        var data = new HalfHalfStrategy().GenerateCard(
            request,
            new List<IProvider>
            {
                new AttackProvider(),
                new EvadeProvider(),
                new ShieldProvider(),
                new InstantMoveProvider(),
                new BayProvider()
            });

        if (DebugMode)
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
            data.CardData.flippable = true;
        }
        
        JesterCLI.Main();

        return data;
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