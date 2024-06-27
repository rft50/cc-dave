using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace InfiniteMode.Features;

[HarmonyPatch]
public class CorruptedArtifactManager
{
    public static readonly CorruptedArtifactManager Instance = new();

    public void Register()
    {
    }

    public bool IsArtifactCorrupted(Artifact a)
    {
        return ModEntry.Instance.KokoroApi.TryGetExtensionData<CorruptedArtifactStatus>(a, "corruptedArtifact", out var data)
               && data != CorruptedArtifactStatus.Normal;
    }
    
    public List<Artifact> GetCorruptedArtifacts(State s)
    {
        return s.EnumerateAllArtifacts()
            .Where(IsArtifactCorrupted)
            .ToList();
    }

    public List<Artifact> GetCorruptibleArtifacts(State s)
    {
        return s.EnumerateAllArtifacts()
            .Where(a => !DB.artifactMetas[a.Key()].unremovable)
            .Where(a => !Instance.IsArtifactCorrupted(a))
            .ToList();
    }
    
    public void SetArtifactCorruption(Artifact a, CorruptedArtifactStatus status)
    {
        ModEntry.Instance.KokoroApi.SetExtensionData(a, "corruptedArtifact", status);
    }

    [HarmonyPatch(typeof(Artifact), "GetTooltips")]
    [HarmonyPostfix]
    private static void Artifact_GetTooltips_Postfix(Artifact __instance, List<Tooltip> __result)
    {
        if (Instance.IsArtifactCorrupted(__instance))
        {
            __result.Add(new TTText("<c=downside>This artifact is Corrupted.</c>"));
        }
    }
}

public enum CorruptedArtifactStatus
{
    Normal,
    Zone1,
    Zone2
}