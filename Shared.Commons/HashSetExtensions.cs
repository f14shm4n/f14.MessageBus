namespace Shared.Commons
{
    public static partial class CollectionExtensions
    {
        public static void AddRange<T>(this HashSet<T> source, IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(items);

            foreach (var item in items)
            {
                source.Add(item);
            }
        }
    }
}
