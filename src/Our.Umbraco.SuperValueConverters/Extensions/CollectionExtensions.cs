using System.Collections.Generic;
using System.Linq;

namespace Our.Umbraco.SuperValueConverters.Extensions
{
    internal static class CollectionExtensions
    {
        public static IEnumerable<T> IntersectMany<T>(this IEnumerable<IEnumerable<T>> values)
        {
            IEnumerable<T> intersection = [];

            foreach (var value in values)
            {
                if (intersection?.Any() != true)
                {
                    intersection = [..value];
                }
                else
                {
                    intersection = intersection.Intersect(value);
                }
            }

            return intersection;
        }
    }
}