namespace Dave.External;

public partial interface IJesterApi
{
    public IJesterUtil GetJesterUtil();

    public interface IJesterUtil
    {
        // Inclusive on both sides
        public bool InRange(int min, int val, int max);

        public bool ContainsAll<T>(IEnumerable<T> source, IEnumerable<T> mustContain);

        public T GetRandom<T>(IList<T> source, Rand rng);

        // Generates a list of legal positions to deploy midrow objects
        // They are legal in the sense that they would not collide with another deployment on the card
        // If occupied does not contain offset and skip is 0, only a list with offset will be returned
        // Otherwise, the earliest leftmost and rightmost legal positions will be generated
        // If skip is specified, the first X legal locations will be skipped
        public IList<int> GetDeployOptions(ISet<int> occupied, int offset = 0, int skip = 0);

        // Fisher-Yates Shuffle
        public void Shuffle<T>(IList<T> list, Rand rng);
    }
}