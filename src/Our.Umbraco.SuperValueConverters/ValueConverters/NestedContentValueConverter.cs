using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Core = Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class NestedContentValueConverter : SuperValueConverterBase
    {
        private readonly ModelFactory _modelFactory;

        public NestedContentValueConverter(Core.NestedContentManyValueConverter valueConverter, ModelFactory modelFactory)
            : base(valueConverter, modelFactory)
        {
            _modelFactory = modelFactory;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals(Constants.PropertyEditors.Aliases.NestedContent);
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            var config = propertyType.DataType.ConfigurationAs<NestedContentConfiguration>();

            var allowedTypes = config.ContentTypes?.Select(x => x.Alias).ToArray() ?? new string[0];

            var multiple = config.MaxItems != 1;

            var itemType = _modelFactory.GetModelType(allowedTypes) ?? typeof(IPublishedElement);

            if (multiple == true)
            {
                return typeof(IEnumerable<>).MakeGenericType(itemType);
            }

            return itemType;
        }
    }
}