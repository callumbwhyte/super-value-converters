using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.SuperValueConverters.Helpers
{
    internal class TypeHelper
    {
        public static IEnumerable<Type> GetPublishedModelTypes(string[] typeNames)
        {
            var typeLoader = Current.Factory.GetInstance<TypeLoader>();

            var types = typeLoader.GetTypes<IPublishedElement>();

            foreach (var typeName in typeNames)
            {
                var type = types.FirstOrDefault(x =>
                {
                    var attribute = x.GetCustomAttribute<PublishedModelAttribute>(false);

                    var modelName = attribute != null ? attribute.ContentTypeAlias : x.Name;

                    return typeName.Equals(modelName, StringComparison.OrdinalIgnoreCase);
                });

                if (type != null)
                {
                    yield return type;
                }
            }
        }
    }
}