using System.Collections.Generic;
using System.Reflection;
using Our.Umbraco.SuperValueConverters.PreValues.Attributes;

namespace Our.Umbraco.SuperValueConverters.PreValues
{
    public class PreValueMapper
    {
        public static T Map<T>(T model, IDictionary<string, string> preValues)
            where T : class
        {
            var properties = model.GetType().GetProperties();

            foreach (var property in properties)
            {
                var preValueProperty = property.GetCustomAttribute<PreValuePropertyAttribute>();

                if (preValueProperty != null)
                {
                    var value = preValues[preValueProperty.Alias];

                    var preValueFilter = property.GetCustomAttribute<PreValueFilterAttribute>(true);

                    if (preValueFilter != null)
                    {
                        property.SetValue(model, preValueFilter.Process(value));
                    }
                    else
                    {
                        if (property.PropertyType == typeof(bool))
                        {
                            property.SetValue(model, ConvertToBoolean(value));
                        }

                        if (property.PropertyType == typeof(int))
                        {
                            property.SetValue(model, ConvertToInt(value));
                        }

                        if (property.PropertyType == typeof(string[]))
                        {
                            property.SetValue(model, ConvertToStringArray(value));
                        }
                    }
                }
            }

            return model;
        }

        private static bool ConvertToBoolean(string input)
        {
            bool value = false;

            bool.TryParse(input, out value);

            return value;
        }

        private static int ConvertToInt(string input)
        {
            int value = 0;

            int.TryParse(input, out value);

            return value;
        }

        private static string[] ConvertToStringArray(string input, char separator = ',')
        {
            return input.Replace(" ", "").Split(separator);
        }
    }
}