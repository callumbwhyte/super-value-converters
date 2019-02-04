using System.Collections;

namespace Our.Umbraco.SuperValueConverters.Extensions
{
    public static class IListExtensions
    {
        public static object FirstOrNull(this IList source)
        {
            return source.Count > 0 ? source[0] : null;
        }
    }
}