using System;
using System.Collections;
using System.Linq;

namespace Our.Umbraco.SuperValueConverters.Helpers
{
    public class TypeHelper
    {
        public static Type GetType(string typeName)
        {
            foreach (var assembley in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembley.GetTypes())
                {
                    if (type.Name.Equals(typeName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return type;
                    }
                }
            }

            return null;
        }

        public static bool IsIEnumerable(Type type)
        {
            return type.IsAssignableFrom(typeof(IEnumerable));
        }

        public static Type GetInnerType(Type type)
        {
            return type.GetGenericArguments().FirstOrDefault();
        }
    }
}