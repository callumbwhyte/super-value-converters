using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
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

        public static IPickerSettings GetMNTPSettings(int dataTypeId)
        {
            var preValues = GetPreValues(dataTypeId);

            if (preValues.Any() == true)
            {
                var allowedDoctypes = preValues["filter"].Replace(" ", "").Split(',');
                var maxItems = Convert.ToInt32(preValues["maxNumber"]);

                return new MNTPSettings
                {
                    AllowedDoctypes = allowedDoctypes,
                    MaxItems = maxItems
                };
            }

            return null;
        }

        public static IPickerSettings GetNestedContentSettings(int dataTypeId)
        {
            var preValues = GetPreValues(dataTypeId);

            if (preValues.Any() == true)
            {
                var contentTypesJson = JArray.Parse(preValues["contentTypes"]);

                var allowedDoctypes = contentTypesJson
                    .Select(x => x.Value<string>("ncAlias"))
                    .ToArray();

                var maxItems = Convert.ToInt32(preValues["maxItems"]);

                return new NestedContentSettings
                {
                    AllowedDoctypes = allowedDoctypes,
                    MaxItems = maxItems
                };
            }

            return null;
        }

        public static IPickerSettings GetMediaPickerSettings(int dataTypeId)
        {
            var preValues = GetPreValues(dataTypeId);

            if (preValues.Any() == true)
            {
                var allowedDoctypes = new List<string>();

                if (preValues["onlyImages"] == "1")
                {
                    allowedDoctypes.Add("Image");
                }

                var maxItems = preValues["multiPicker"] == "1" ? 0 : 1;

                return new MediaPickerSettings
                {
                    AllowedDoctypes = allowedDoctypes.ToArray(),
                    MaxItems = maxItems
                };
            }

            return null;
        }
    }
}