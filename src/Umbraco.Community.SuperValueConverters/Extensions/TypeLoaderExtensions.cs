using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Umbraco.Community.SuperValueConverters.Extensions;

internal static class TypeLoaderExtensions
{
    public static Type? FindCommonInterfaces(this TypeLoader typeLoader, IEnumerable<Type> types, Func<Type, bool>? predicate = null)
    {
        if (types.Count() > 1)
        {
            var interfaces = types.Select(x => x.GetInterfaces().Where(y => y.IsPublic && (predicate == null || predicate(y))));

            var sharedInterfaces = interfaces.IntersectMany();

            return sharedInterfaces.LastOrDefault();
        }

        return types.FirstOrDefault();
    }

    public static IEnumerable<Type> GetPublishedModelTypes(this TypeLoader typeLoader, params string[] contentTypeAliases)
    {
        return typeLoader
            .GetTypes<IPublishedElement>()
            .Where(type =>
            {
                var attribute = type.GetCustomAttribute<PublishedModelAttribute>(false);

                var modelName = attribute?.ContentTypeAlias ?? type.Name;

                return contentTypeAliases.InvariantContains(modelName);
            });
    }
}