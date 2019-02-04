using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Our.Umbraco.SuperValueConverters.Helpers;
using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class NestedContentValueConverter : NestedContentManyValueConverter, IPropertyValueConverterMeta
    {
        public override PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType, PropertyCacheValue cacheValue)
        {
            return BaseValueConverter.GetPropertyCacheLevel(propertyType, cacheValue);
        }

        public override Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            var pickerSettings = GetSettings(propertyType);

            return BaseValueConverter.GetPropertyValueType(propertyType, pickerSettings);
        }

        public override object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            var value = base.ConvertSourceToObject(propertyType, source, preview) as IEnumerable<IPublishedContent>;

            return BaseValueConverter.ConvertSourceToObject(propertyType, value);
        }

        private IPickerSettings GetSettings(PublishedPropertyType propertyType)
        {
            var preValues = DataTypeHelper.GetPreValues(propertyType.DataTypeId);

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
    }
}