using System;
using System.Collections;
using System.Linq;

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
                types = types.Where(x => x.Namespace.Equals(namespaceName, StringComparison.InvariantCultureIgnoreCase));
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

        public static bool IsIEnumerable(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }
    }
}