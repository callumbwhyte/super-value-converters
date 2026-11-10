using System.Collections.Generic;
using System.Linq;

namespace Our.Umbraco.SuperValueConverters.Extensions
{
    internal static class IEnumerableExtensions
    {
        public static IEnumerable<T> IntersectMany<T>(this IEnumerable<IEnumerable<T>> values)
        {
            IEnumerable<T> intersection = [];

            foreach (var value in values)
            {
                if (intersection == null)
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