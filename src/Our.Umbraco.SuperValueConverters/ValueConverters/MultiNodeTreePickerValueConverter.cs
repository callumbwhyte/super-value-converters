using System;
using Our.Umbraco.SuperValueConverters.Helpers;
using Our.Umbraco.SuperValueConverters.Models;
using Our.Umbraco.SuperValueConverters.PreValues;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;
using Umbraco.Web.PublishedCache;
using Core = Umbraco.Web.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class MultiNodeTreePickerValueConverter : Core.MultiNodeTreePickerValueConverter
    {
        public MultiNodeTreePickerValueConverter(IPublishedSnapshotAccessor publishedSnapshotAccessor)
            : base(publishedSnapshotAccessor)
        {

        }

        public override Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            var pickerSettings = GetSettings(propertyType);

            return BaseValueConverter.GetPropertyValueType(propertyType, pickerSettings);
        }

        public override object ConvertIntermediateToObject(IPublishedElement owner, PublishedPropertyType propertyType, PropertyCacheLevel cacheLevel, object source, bool preview)
        {
            var value = base.ConvertIntermediateToObject(owner, propertyType, cacheLevel, source, preview);

            return BaseValueConverter.ConvertIntermediateToObject(propertyType, value);
        }

        private IPickerSettings GetSettings(PublishedPropertyType propertyType)
        {
            var preValues = DataTypeHelper.GetPreValues(propertyType.DataTypeId);

            var settings = new MNTPSettings();

            return PreValueMapper.Map(settings, preValues);
        }
    }
}