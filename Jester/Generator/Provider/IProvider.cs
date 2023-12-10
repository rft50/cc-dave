namespace Jester.Generator.Provider;

public interface IProvider
{
    // cost bounds are inclusive
    public List<IEntry> GetEntries(JesterRequest request);
}