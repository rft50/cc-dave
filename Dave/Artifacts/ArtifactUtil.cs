namespace Dave.Artifacts;

internal class ArtifactUtil
{
    public static bool PlayerHasArtifact(State s, Artifact a)
    {
        return s.EnumerateAllArtifacts().Contains(a);
    }

    public static bool PlayerHasArtifactOfType(State s, Type t)
    {
        return s.EnumerateAllArtifacts().Any(a => a.GetType() == t);
    }
}