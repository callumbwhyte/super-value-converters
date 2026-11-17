using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    [HideFromTypeFinder]
    internal class MemberPickerValueConverter<T> : SuperValueConverterBase
        where T : IPropertyValueConverter
    {
        private readonly IMemberTypeService _memberTypeService;

        public MemberPickerValueConverter(T valueConverter, TypeLoader typeLoader, IMemberTypeService memberTypeService)
            : base(valueConverter, typeLoader)
        {
            _memberTypeService = memberTypeService;
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            string? filter = null;

            var multiple = false;

            var config = propertyType.DataType.ConfigurationObject;

#if NET11_0_OR_GREATER
            if (config is MultipleMemberPickerConfiguration multi)
            {
                filter = multi.Filter;

                multiple = multi.ValidationLimit?.Max != 1;
            }
            else if (config is MemberPickerConfiguration single)
            {
                filter = single.Filter;
            }
#endif

            var keys = filter?
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Guid.TryParse(x, out var guid) ? guid : default);

            var itemType = GetItemType(keys ?? []);

            if (multiple == true)
            {
                return typeof(IEnumerable<>).MakeGenericType(itemType);
            }

            return itemType;
        }

        private Type GetItemType(IEnumerable<Guid> keys)
        {
            var aliases = _memberTypeService
                .GetMany(keys)
                .Select(x => x.Alias)
                .ToArray();

            if (aliases?.Any() == true)
            {
                var modelType = GetPublishedModelType(aliases);

                if (modelType != null)
                {
                    return modelType;
                }
            }

            return typeof(IPublishedContent);
        }
    }
}