using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Core = Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class MultiNodeTreePickerValueConverter : SuperValueConverterBase
    {
        private readonly IPropertyValueConverter _valueConverter;

        public MultiNodeTreePickerValueConverter(Core.MultiNodeTreePickerValueConverter valueConverter, TypeLoader typeLoader)
            : base(valueConverter, typeLoader)
        {
            _valueConverter = valueConverter;
        }

        public override string[] IgnoreProperties => new[]
        {
            Constants.Conventions.Content.InternalRedirectId,
            Constants.Conventions.Content.Redirect
        };

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            if (IgnoreProperties.Contains(propertyType.Alias) == true)
            {
                return _valueConverter.GetPropertyValueType(propertyType);
            }

            var config = propertyType.DataType.ConfigurationAs<MultiNodePickerConfiguration>();

            var allowedTypes = config.Filter?.Split(',') ?? new string[0];

            var multiple = config.MaxNumber != 1;

            var itemType = GetPublishedModelType(allowedTypes) ?? typeof(IPublishedContent);

            if (multiple == true)
            {
                return typeof(IEnumerable<>).MakeGenericType(itemType);
            }

            return itemType;
        }
    }
}