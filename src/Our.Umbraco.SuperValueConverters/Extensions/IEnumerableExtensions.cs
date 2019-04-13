using System.Collections.Generic;
using System.Linq;

namespace Our.Umbraco.SuperValueConverters.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> IntersectMany<T>(this IEnumerable<IEnumerable<T>> values)
        {
            IEnumerable<T> intersection = null;

            foreach (var value in values)
            {
                if (intersection == null)
                {
                    intersection = new List<T>(value);
                }
                else
                {
                    intersection.Intersect(value);
                }
            }

            return intersection ?? Enumerable.Empty<T>();
        }
    }
}