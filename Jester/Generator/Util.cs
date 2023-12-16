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
        } while (leftSkip > 0 || occupied.Contains(left));

        do
        {
            right++;
            if (occupied.Contains(right))
                continue;
            rightSkip--;
        } while (rightSkip > 0 || occupied.Contains(right));

        return new List<int>
        {
            left,
            right
        };
    }

    public static void Shuffle<T>(List<T> list, Random rng)
    {
        for (var i = 0; i < list.Count - 1; i++)
        {
            var j = rng.Next(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}