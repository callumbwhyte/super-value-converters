namespace Umbraco.Community.SuperValueConverters.Extensions;

internal static class CollectionExtensions
{
    public static IEnumerable<T> IntersectMany<T>(this IEnumerable<IEnumerable<T>> values)
    {
        IEnumerable<T>? intersection = null;

        foreach (var value in values)
        {
            if (intersection == null)
            {
                intersection = [.. value];
            }
            else
            {
                intersection = intersection.Intersect(value);
            }
        }

        return intersection ?? [];
    }
}