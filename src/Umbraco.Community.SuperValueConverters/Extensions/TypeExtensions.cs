using System.Collections;

namespace Umbraco.Community.SuperValueConverters.Extensions;

internal static class TypeExtensions
{
    public static IList CreateList(this Type type)
    {
        var listType = typeof(List<>).MakeGenericType(type);

        return (IList)Activator.CreateInstance(listType)!;
    }

    public static Type GetInnerType(this Type type)
    {
        return type.GetGenericArguments().Single();
    }

    public static bool IsIEnumerable(this Type type)
    {
        return typeof(IEnumerable).IsAssignableFrom(type);
    }
}