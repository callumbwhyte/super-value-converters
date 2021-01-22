using System;
using System.Collections.Generic;
using System.Linq;

namespace Our.Umbraco.SuperValueConverters.Helpers
{
    internal class TypeHelper
    {
        public static Type GetType(string typeName, string namespaceName = null)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x != null)
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
            var types = typeNames
                .Select(x => GetType(x, namespaceName))
                .Where(x => x != null);

            return types ?? Enumerable.Empty<Type>();
        }
    }
}