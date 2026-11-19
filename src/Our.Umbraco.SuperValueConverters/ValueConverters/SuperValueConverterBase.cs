using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.SuperValueConverters.Extensions;
using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public abstract class SuperValueConverterBase : PropertyValueConverterBase
    {
        private readonly IPropertyValueConverter _valueConverter;
        private readonly ModelFactory _modelFactory;

        public SuperValueConverterBase(IPropertyValueConverter valueConverter, ModelFactory modelFactory)
        {
            _valueConverter = valueConverter;
            _modelFactory = modelFactory;
        }

        /// <summary>
        /// Property aliases to ignore, using the Core value converter instead
        /// </summary>
        public virtual string[] IgnoreProperties { get; }

        /// <summary>
        /// Converts an individual item to the given type
        /// </summary>
        protected virtual object ConvertItemToType(object item, Type type) => null;

        /// <summary>
        /// Builds the <see cref="IPickerSettings" /> object for the property type
        /// </summary>
        [Obsolete("Implement GetPropertyValueType within the value converter instead")]
        public virtual IPickerSettings GetSettings(IPublishedPropertyType propertyType) => null;

        /// <inheritdoc />
        public override bool IsConverter(IPublishedPropertyType propertyType)
        {
            return _valueConverter.IsConverter(propertyType);
        }

        public override bool? IsValue(object value, PropertyValueLevel level)
        {
            return _valueConverter.IsValue(value, level);
        }

        /// <inheritdoc />
        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            var settings = GetSettings(propertyType);

            var modelType = settings?.BaseType ?? typeof(IPublishedElement);

            if (settings?.AllowedTypes?.Any() == true)
            {
                modelType = _modelFactory.GetModelType(settings.AllowedTypes.ToArray()) ?? settings.BaseType;
            }

            if (settings?.AllowsMultiple() == true)
            {
                return typeof(IEnumerable<>).MakeGenericType(modelType);
            }

            return modelType;
        }

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType)
        {
            return _valueConverter.GetPropertyCacheLevel(propertyType);
        }

        /// <inheritdoc />
        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {
            return _valueConverter.ConvertSourceToIntermediate(owner, propertyType, source, preview);
        }

        /// <inheritdoc />
        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            var value = _valueConverter.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);

            if (IgnoreProperties.InvariantContains(propertyType.Alias) == true)
            {
                return value;
            }

            var clrType = propertyType.ClrType ?? GetPropertyValueType(propertyType);

            var multiple = clrType.IsIEnumerable();

            var itemType = multiple ? clrType.GetInnerType() : clrType;

            var result = itemType.CreateList();

            var items = value as IEnumerable ?? new[] { value };

            foreach (var item in items)
            {
                if (itemType.IsAssignableFrom(item?.GetType()) == true)
                {
                    result.Add(item);
                }
                else
                {
                    var converted = ConvertItemToType(item, itemType);

                    if (converted != null)
                    {
                        result.Add(converted);
                    }
                }
            }

            return multiple ? result : result.FirstOrNull();
        }

        /// <inheritdoc />
        public override object ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            return _valueConverter.ConvertIntermediateToXPath(owner, propertyType, referenceCacheLevel, inter, preview);
        }
    }
}