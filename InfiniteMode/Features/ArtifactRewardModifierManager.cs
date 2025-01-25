using System.Linq;
using HarmonyLib;
using InfiniteMode.Artifacts;

namespace InfiniteMode.Features;

[HarmonyPatch]
public class ArtifactRewardModifierManager
{
    public static readonly ArtifactRewardModifierManager Instance = new();

    public void Register()
    {
    }

    [HarmonyPatch(typeof(Combat), "PlayerWon")]
    [HarmonyPostfix]
    private static void Combat_PlayerWon_Postfix(G g, Combat __instance)
    {
        var s = g.state;
        if (!s.EnumerateAllArtifacts().Any(a => a is InfinityArtifact)) return;

        var reward = s.rewardsQueue.FirstOrDefault(a => a is AArtifactOffering) as AArtifactOffering;
        if (reward is null || reward.limitPools?.Count > 1) return;
        if (reward.limitPools is not null && reward.limitPools[0] == ArtifactPool.Boss && s.map.GetType() != typeof(MapThree))
        {
            reward.limitPools[0] = ArtifactPool.Common;
        }
        else if (reward.limitPools is null || reward.limitPools[0] == ArtifactPool.Common)
        {
            s.rewardsQueue.Remove(reward);
        }
    }
}