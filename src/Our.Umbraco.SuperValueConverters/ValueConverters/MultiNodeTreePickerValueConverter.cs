using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Core = Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class MultiNodeTreePickerValueConverter : SuperValueConverterBase
    {
        private readonly IPropertyValueConverter _valueConverter;
        private readonly ModelFactory _modelFactory;

        public MultiNodeTreePickerValueConverter(Core.MultiNodeTreePickerValueConverter valueConverter, ModelFactory modelFactory)
            : base(valueConverter, modelFactory)
        {
            _valueConverter = valueConverter;
            _modelFactory = modelFactory;
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

            var itemType = _modelFactory.GetModelType(allowedTypes) ?? typeof(IPublishedContent);

            if (multiple == true)
            {
                return typeof(IEnumerable<>).MakeGenericType(itemType);
            }

            return itemType;
        }
    }
}