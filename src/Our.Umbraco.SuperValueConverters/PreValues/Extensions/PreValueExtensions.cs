using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;

namespace Our.Umbraco.SuperValueConverters.PreValues.Extensions
{
    public static class PreValueExtensions
    {
        public static T Map<T>(this PreValueCollection preValues)
            where T : class, new()
        {
            var preValuesDictionary = preValues.PreValuesAsDictionary;

            return preValuesDictionary.Map<T>();
        }

        public static T Map<T>(this IDictionary<string, PreValue> preValues)
            where T : class, new()
        {
            var preValuesDictionary = preValues.ToDictionary(x => x.Key, x => x.Value.Value);

            return preValuesDictionary.Map<T>();
        }

        public static T Map<T>(this IDictionary<string, string> preValues)
            where T : class, new()
        {
            var mapped = PreValueMapper.Map(new T(), preValues);

            return mapped;
        }
    }
}