using System.Collections;

namespace Umbraco.Community.SuperValueConverters.Extensions;

internal static class ListExtensions
{
    public static object? FirstOrNull(this IList? source)
    {
        return source?.Count > 0 ? source[0] : null;
    }
}