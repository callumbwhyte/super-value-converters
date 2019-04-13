using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;

namespace Our.Umbraco.SuperValueConverters.Helpers
{
    public class TypeHelper
    {
        public static Type GetType(string typeName, string namespaceName = null)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass);

            if (string.IsNullOrEmpty(namespaceName) == false)
            {
                types = types
                    .Where(x => x.Namespace != null)
                    .Where(x => x.Namespace.Equals(namespaceName, StringComparison.InvariantCultureIgnoreCase));
            }

            foreach (var type in types)
            {
                if (type.Name.Equals(typeName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return type;
                }
            }

            return null;
        }

        public static IEnumerable<Type> GetTypes(string[] typeNames, string namespaceName = null)
        {
            return typeNames.Select(x => GetType(x, namespaceName));
        }

        public static bool IsIEnumerable(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static bool IsIPublishedContent(Type type)
        {
            return typeof(IPublishedContent).IsAssignableFrom(type);
        }

        public static Type GetInnerType(Type type)
        {
            return type.GetGenericArguments().FirstOrDefault();
        }

        public static IList CreateListOfType(Type type)
        {
            var listType = typeof(List<>).MakeGenericType(type);

            return (IList)Activator.CreateInstance(listType);
        }
    }
}