using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;
using Core = Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class MultiNodeTreePickerValueConverter : SuperValueConverterBase
    {
    
        private static readonly List<string> PropertiesToExclude = new List<string>
        {
            Constants.Conventions.Content.InternalRedirectId.ToLower(CultureInfo.InvariantCulture),
            Constants.Conventions.Content.Redirect.ToLower(CultureInfo.InvariantCulture)
        };
    
        public MultiNodeTreePickerValueConverter(Core.MultiNodeTreePickerValueConverter baseValueConverter, TypeLoader typeLoader)
            : base(baseValueConverter, typeLoader)
        {

        }

        public override IPickerSettings GetSettings(IPublishedPropertyType propertyType)
        {
            var configuration = propertyType.DataType.ConfigurationAs<MultiNodePickerConfiguration>();

            var settings = new PickerSettings
            {
                MaxItems = configuration.MaxNumber
            };

            if (string.IsNullOrEmpty(configuration.Filter) == false)
            {
                settings.AllowedTypes = configuration.Filter.Split(',');
            }

            return settings;
        }
        
        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            if ((propertyType.Alias == null || PropertiesToExclude.InvariantContains(propertyType.Alias)) )
            {
                var udis = (Udi[])inter;

                return udis.FirstOrDefault();
            } 

            return base.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);
        }
    }
}
