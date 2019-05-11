using System;
using Our.Umbraco.SuperValueConverters.Helpers;
using Our.Umbraco.SuperValueConverters.Models;
using Our.Umbraco.SuperValueConverters.PreValues;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;
using Umbraco.Web.PublishedCache;
using Core = Umbraco.Web.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class NestedContentValueConverter : Core.NestedContentManyValueConverter
    {
        public NestedContentValueConverter(IPublishedSnapshotAccessor publishedSnapshotAccessor, IPublishedModelFactory publishedModelFactory, IProfilingLogger proflog)
            : base(publishedSnapshotAccessor, publishedModelFactory, proflog)
        {

        }

        public override Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            var pickerSettings = GetSettings(propertyType);

            return BaseValueConverter.GetPropertyValueType(propertyType, pickerSettings);
        }

        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals(Constants.PropertyEditors.Aliases.NestedContent);
        }

        public override object ConvertIntermediateToObject(IPublishedElement owner, PublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            var value = base.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);

            return BaseValueConverter.ConvertIntermediateToObject(propertyType, value);
        }

        private IPickerSettings GetSettings(PublishedPropertyType propertyType)
        {
            var preValues = DataTypeHelper.GetPreValues(propertyType.DataTypeId);

            var settings = new NestedContentSettings();

            return PreValueMapper.Map(settings, preValues);
        }
    }
}