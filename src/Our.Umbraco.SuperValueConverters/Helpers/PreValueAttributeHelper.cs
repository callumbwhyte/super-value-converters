using System.Collections.Generic;
using System.Reflection;
using Our.Umbraco.SuperValueConverters.Attributes;
using Our.Umbraco.SuperValueConverters.Models;

namespace Our.Umbraco.SuperValueConverters.Helpers
{
    public class PreValueAttributeHelper
    {
        public static IPickerSettings Map(IPickerSettings pickerSettings, IDictionary<string, string> preValues)
        {
            var properties = pickerSettings.GetType().GetProperties();

            foreach (var property in properties)
            {
                var preValueProperty = property.GetCustomAttribute<PreValuePropertyAttribute>();

                if (preValueProperty != null)
                {
                    var value = preValues[preValueProperty.Alias];

                    var preValueFilter = property.GetCustomAttribute<PreValueFilterAttribute>(true);

                    if (preValueFilter != null)
                    {
                        property.SetValue(pickerSettings, preValueFilter.Process(value));
                    }

                    if (property.PropertyType == typeof(bool))
                    {
                        property.SetValue(pickerSettings, ConvertToBoolean(value));
                    }

                    if (property.PropertyType == typeof(int))
                    {
                        property.SetValue(pickerSettings, ConvertToInt(value));
                    }

                    if (property.PropertyType == typeof(string[]))
                    {
                        property.SetValue(pickerSettings, ConvertToStringArray(value));
                    }
                }
            }

            return null;
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