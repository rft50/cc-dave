using System;
using System.Collections.Generic;
using System.Linq;
using InfiniteMode.ExternalAPI;
using InfiniteMode.Features.RestartOption;
using InfiniteMode.Map;

namespace InfiniteMode.Features;

public class RestartOptionManager
{
    public static readonly RestartOptionManager Instance = new();

    private readonly List<IRestartOption> _restartOptions = [
        new CardCorruption(),
        new ArtifactCorruption(),
        new CoreCorruption(),
        new HullDissolution()
    ];

    public void Register()
    {
        DB.story.all["infinite_loopRestart"] = new()
        {
            type = NodeType.@event,
            bg = "BGBootSequence",
            canSpawnOnMap = false,
            zones = [],
            lines = [
                new CustomSay
                {
                    who = "comp",
                    loopTag = "loading3",
                    Text = ModEntry.Instance.Localizations.Localize(["restart", "boot", "text"])
                }
            ],
            choiceFunc = "infinite_loopRestart"
        };
        DB.eventChoiceFns.Add("infinite_loopRestart", ((Func<State, List<Choice>>)MapLoopRestart.GetChoices).Method);
    }

    public List<Choice> GenerateChoices(State s, int choiceCount)
    {
        var options = _restartOptions
            .Where(ro => ro.Selectable(s))
            .SelectMany<IRestartOption, WeightedItem<IRestartOption>>(ro =>
            {
                var weight = ro.GetWeight(s);
                return
                [
                    new WeightedItem<IRestartOption>(weight, ro),
                    new WeightedItem<IRestartOption>(weight, ro)
                ];
            }).ToList();
        var weightedOptions = new WeightedRandom<IRestartOption>(options);
        var rand = s.rngCurrentEvent.Offshoot();
        
        return Enumerable.Range(0, Math.Min(choiceCount, options.Count / 2))
            .Select(i => GenerateChoice(s, weightedOptions.Next(rand, true), weightedOptions.Next(rand, true)))
            .ToList();
    }

    private Choice GenerateChoice(State s, IRestartOption first, IRestartOption second)
    {
        if (first.GetType() == second.GetType())
        {
            return new Choice
            {
                label = first.GetDoubleDescription(s),
                actions = [
                    new AShipUpgrades
                    {
                        actions = first.GetDoubleActions(s)
                    }
                ]
            };
        }
        else
        {
            return new Choice
            {
                label = ModEntry.Instance.Localizations.Localize(["restart", "boot", "pair"], new {First = first.GetSingleDescription(s), Second = second.GetSingleDescription(s)}),
                actions = [
                    new AShipUpgrades
                    {
                        actions = first.GetSingleActions(s).Concat(second.GetSingleActions(s)).ToList()
                    }
                ]
            };
        }
    }
}