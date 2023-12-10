namespace Jester.Generator;

public class Util
{
    // inclusive
    public static bool InRange(int min, int val, int max)
    {
        return val >= min && val <= max;
    }

    public static bool ContainsAll<T>(IEnumerable<T> source, IEnumerable<T> mustContain)
    {
        return mustContain.All(source.Contains);
    }

    public static T GetRandom<T>(List<T> source, Random rng)
    {
        return source[rng.Next(source.Count)];
    }

    public static List<int> GetDeployOptions(HashSet<int> occupied, int offset = 0, int skip = 0)
    {
        if (!occupied.Contains(offset))
            return new List<int> { offset };

        var left = offset;
        var leftSkip = skip;
        var right = offset;
        var rightSkip = skip;

        do
        {
            left--;
            if (occupied.Contains(left))
                continue;
            leftSkip--;
        } while (leftSkip > 0);

        do
        {
            right++;
            if (occupied.Contains(right))
                continue;
            rightSkip--;
        } while (rightSkip > 0);

        return new List<int>
        {
            left,
            right
        };
    }
}