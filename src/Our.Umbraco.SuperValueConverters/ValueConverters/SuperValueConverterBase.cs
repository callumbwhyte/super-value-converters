using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.SuperValueConverters.Extensions;
using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public abstract class SuperValueConverterBase : PropertyValueConverterBase
    {
        private readonly IPropertyValueConverter _valueConverter;
        private readonly TypeLoader _typeLoader;

        public SuperValueConverterBase(IPropertyValueConverter valueConverter, TypeLoader typeLoader)
        {
            _valueConverter = valueConverter;
            _typeLoader = typeLoader;
        }

        /// <summary>
        /// Property aliases to ignore, using the Core value converter instead
        /// </summary>
        public virtual string[] IgnoreProperties { get; }

        /// <summary>
        /// Builds the <see cref="IPickerSettings" /> object for the property type
        /// </summary>
        [Obsolete("Implement GetPropertyValueType within the value converter instead")]
        public virtual IPickerSettings GetSettings(IPublishedPropertyType propertyType) => null;

        /// <summary>
        /// Gets the common published model type for the given content type aliases
        /// </summary>
        protected virtual Type GetPublishedModelType(params string[] contentTypeAliases)
        {
            var types = _typeLoader.GetPublishedModelTypes(contentTypeAliases);

            return _typeLoader.FindCommonInterfaces(types, type =>
            {
                return type.Namespace != typeof(IPublishedElement).Namespace;
            });
        }

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
                modelType = GetPublishedModelType(settings.AllowedTypes.ToArray()) ?? settings.BaseType;
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

            var allowsMultiple = clrType.IsIEnumerable();

            var modelType = allowsMultiple == true ? clrType.GetInnerType() : clrType;

            var result = modelType.CreateList();

            if (value is IEnumerable items)
            {
                foreach (var item in items)
                {
                    if (item != null)
                    {
                        if (modelType.IsAssignableFrom(item.GetType()) == true)
                        {
                            result.Add(item);
                        }
                    }
                }
            }
            else if (value != null)
            {
                if (modelType.IsAssignableFrom(value.GetType()) == true)
                {
                    result.Add(value);
                }
            }

            return allowsMultiple == true ? result : result.FirstOrNull();
        }

        /// <inheritdoc />
        public override object ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            return _valueConverter.ConvertIntermediateToXPath(owner, propertyType, referenceCacheLevel, inter, preview);
        }
    }
}