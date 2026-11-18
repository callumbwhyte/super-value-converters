using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Community.SuperValueConverters.Extensions;
using Umbraco.Extensions;

namespace Umbraco.Community.SuperValueConverters
{
    public sealed class ModelFactory
    {
        private readonly IPublishedModelFactory _modelFactory;
        private readonly IPublishedContentTypeCache _contentTypeCache;
        private readonly IServiceProvider _serviceProvider;

        public ModelFactory(IPublishedModelFactory modelFactory, IPublishedContentTypeCache contentTypeCache, IServiceProvider serviceProvider)
        {
            _modelFactory = modelFactory;
            _contentTypeCache = contentTypeCache;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates an instance of a given model type
        /// </summary>
        public object? CreateModel(Type type, params object?[] parameters)
        {
            return _serviceProvider.CreateInstance(type, parameters!);
        }

        /// <summary>
        /// Gets the model type for a given content type alias
        /// </summary>
        public Type? GetModelType(string alias)
        {
            return _modelFactory.GetModelType(alias);
        }

        /// <summary>
        /// Gets the model type for a given <c>PublishedItemType</c> and content type key
        /// </summary>
        public Type? GetModelType(PublishedItemType itemType, Guid key)
        {
            return GetModelTypes(itemType, [key]).FirstOrDefault();
        }

        /// <summary>
        /// Gets the most common model type for a collection of content type aliases
        /// </summary>
        public Type? GetModelType(IEnumerable<string> aliases)
        {
            var types = GetModelTypes(aliases);

            return FindCommonType(types);
        }

        /// <summary>
        /// Gets the model type for a given <c>PublishedItemType</c> and collection of content type keys
        /// </summary>
        public Type? GetModelType(PublishedItemType itemType, IEnumerable<Guid> keys)
        {
            var types = GetModelTypes(itemType, keys);

            return FindCommonType(types);
        }

        /// <summary>
        /// Gets the model types for a collection of content type aliases
        /// </summary>
        public IEnumerable<Type> GetModelTypes(IEnumerable<string> aliases)
        {
            foreach (var alias in aliases)
            {
                var type = GetModelType(alias);

                if (type != null)
                {
                    yield return type;
                }
            }
        }

        /// <summary>
        /// Gets the model types for a given <c>PublishedItemType</c> and collection of content type keys
        /// </summary>
        public IEnumerable<Type> GetModelTypes(PublishedItemType itemType, IEnumerable<Guid> keys)
        {
            foreach (var key in keys)
            {
                var contentType = _contentTypeCache.Get(itemType, key);

                if (contentType == null)
                {
                    continue;
                }

                var type = _modelFactory.GetModelType(contentType.Alias);

                if (type != null)
                {
                    yield return type;
                }
            }
        }

        private static Type? FindCommonType(IEnumerable<Type> types)
        {
            var filter = (Type type) => type.IsPublic && type.Namespace != typeof(IPublishedElement).Namespace;

            if (types.Count() > 1)
            {
                var interfaces = types.Select(x => x.GetInterfaces().Where(filter));

                return interfaces
                    .IntersectMany()
                    .LastOrDefault();
            }

            return types.FirstOrDefault(filter);
        }
    }
}