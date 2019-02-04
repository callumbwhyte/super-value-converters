using System;
using System.Configuration;

namespace Our.Umbraco.SuperValueConverters.Helpers
{
    public class ModelsBuilderHelper
    {
        private const string Prefix = "Umbraco.ModelsBuilder.";

        public static bool IsEnabled()
        {
            var value = ConfigurationManager.AppSettings[Prefix + "Enable"];

            return Convert.ToBoolean(value);
        }

        public static string GetNamespace()
        {
            var value = ConfigurationManager.AppSettings[Prefix + "ModelsNamespace"];

            if (string.IsNullOrWhiteSpace(value) == false)
            {
                return value;
            }

            return "Umbraco.Web.PublishedContentModels";
        }
    }
}