using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Core;

namespace Our.Umbraco.SuperValueConverters.Helpers
{
    public class DataTypeHelper
    {
        public static IDictionary<string, string> GetPreValues(int dataTypeId)
        {
            var dataTypeService = ApplicationContext.Current.Services.DataTypeService;

            var preValues = dataTypeService.GetPreValuesCollectionByDataTypeId(dataTypeId);

            var preValuesDictionary = preValues.PreValuesAsDictionary.ToDictionary(x => x.Key, x => x.Value.Value);

            return preValuesDictionary;
        }

        public static PickerSettings GetPickerSettings(int dataTypeId)
        {
            var preValues = GetPreValues(dataTypeId);

            if (preValues.Any() == true)
            {
                var allowedDoctypes = preValues["filter"].Replace(" ", "").Split(',');
                var maxItems = Convert.ToInt32(preValues["maxNumber"]);

                return new PickerSettings
                {
                    AllowedDoctypes = allowedDoctypes,
                    MaxItems = maxItems
                };
            }

            return null;
        }
    }
}