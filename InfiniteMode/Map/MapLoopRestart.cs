using System.Collections.Generic;
using System.Linq;
using InfiniteMode.Artifacts;
using InfiniteMode.Features;

namespace InfiniteMode.Map;

public class MapLoopRestart : MapNodeContents
{
    public override Route MakeRoute(State s)
    {
        if (!s.EnumerateAllArtifacts().Any(v => v is InfinityArtifact))
            s.AddNonCharacterArtifact(new InfinityArtifact());
        var map = new MapFirst();
        map.Populate(s, s.rngZone.Offshoot());
        s.map = map;
        return Dialogue.MakeDialogueRouteOrSkip(s, DB.story.QuickLookup(s, "infinite_loopRestart"), OnDone.visitCurrent);
    }
    
    public static List<Choice> GetChoices(State s)
    {
        return RestartOptionManager.Instance.GenerateChoices(s, 3);
    }
}