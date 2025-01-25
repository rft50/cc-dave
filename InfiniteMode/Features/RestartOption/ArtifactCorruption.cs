using System.Collections.Generic;
using System.Linq;
using Nickel;

namespace InfiniteMode.Features.RestartOption;

public class ArtifactCorruption : IRestartOption
{
    public bool Selectable(State s) => CorruptedArtifactManager.Instance.GetCorruptibleArtifacts(s).Count >= 4;

    public float GetWeight(State s) => 1;

    public List<CardAction> GetSingleActions(State s) => new List<CardAction>
    {
        new ACorruptArtifact
        {
            Status = CorruptedArtifactStatus.Zone1
        },
        new ACorruptArtifact
        {
            Status = CorruptedArtifactStatus.Zone2
        }
    }.Shuffle(s.rngActions).ToList();

    public List<CardAction> GetDoubleActions(State s) => new List<CardAction>
    {
        new ACorruptArtifact
        {
            Status = CorruptedArtifactStatus.Zone1
        },
        new ACorruptArtifact
        {
            Status = CorruptedArtifactStatus.Zone1
        },
        new ACorruptArtifact
        {
            Status = CorruptedArtifactStatus.Zone2
        },
        new ACorruptArtifact
        {
            Status = CorruptedArtifactStatus.Zone2
        }
    }.Shuffle(s.rngActions).ToList();

    public string GetSingleDescription(State s) => ModEntry.Instance.Localizations.Localize(["restart", "option", "artifact", "single"]);

    public string GetDoubleDescription(State s) => ModEntry.Instance.Localizations.Localize(["restart", "option", "artifact", "double"]);
}

internal class ACorruptArtifact : CardAction
{
    public string Text = ModEntry.Instance.Localizations.Localize(["restart", "option", "artifact", "default"]);
    public CorruptedArtifactStatus Status = CorruptedArtifactStatus.Normal;
    
    public override void Begin(G g, State s, Combat c)
    {
        var candidates = CorruptedArtifactManager.Instance.GetCorruptibleArtifacts(s);
        if (candidates.Count == 0) return;
        var artifact = candidates.ElementAt(s.rngActions.NextInt() % candidates.Count);
        CorruptedArtifactManager.Instance.SetArtifactCorruption(artifact, Status);
        Text = ModEntry.Instance.Localizations.Localize(["restart", "option", "artifact", "chosen"], new {Artifact = artifact.GetLocName()});
    }

    public override string GetUpgradeText(State s) => Text;

    public override List<Tooltip> GetTooltips(State s) =>
    [
        new GlossaryTooltip($"trait.{GetType().Namespace!}::CorruptedArtifact")
        {
            Icon = ModEntry.Instance.CorruptedCardTrait.Configuration.Icon(s, null),
            TitleColor = Colors.action,
            Title = ModEntry.Instance.Localizations.Localize(["trait", "CorruptedArtifact", "name"]),
            Description = ModEntry.Instance.Localizations.Localize(["trait", "CorruptedArtifact", "description"])
        }
    ];
}