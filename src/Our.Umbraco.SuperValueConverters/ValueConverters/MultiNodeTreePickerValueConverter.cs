using System;
using System.Linq;
using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Core = Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class MultiNodeTreePickerValueConverter : SuperValueConverterBase
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IMediaTypeService _mediaTypeService;
        private readonly IMemberTypeService _memberTypeService;

        public MultiNodeTreePickerValueConverter(Core.MultiNodeTreePickerValueConverter baseValueConverter, TypeLoader typeLoader, IContentTypeService contentTypeService, IMediaTypeService mediaTypeService, IMemberTypeService memberTypeService)
            : base(baseValueConverter, typeLoader)
        {
            _contentTypeService = contentTypeService;
            _mediaTypeService = mediaTypeService;
            _memberTypeService = memberTypeService;
        }

        public override string[] IgnoreProperties => new[]
        {
            Constants.Conventions.Content.InternalRedirectId,
            Constants.Conventions.Content.Redirect
        };

        public override IPickerSettings GetSettings(IPublishedPropertyType propertyType)
        {
            var configuration = propertyType.DataType.ConfigurationAs<MultiNodePickerConfiguration>();

            var settings = new PickerSettings
            {
                MaxItems = configuration.MaxNumber
            };

            if (string.IsNullOrEmpty(configuration.Filter) == false)
            {
                var keys = configuration.Filter.Split(Constants.CharArrays.Comma, StringSplitOptions.RemoveEmptyEntries);

                // A filter that can't be read isn't narrowed, as narrowing to the readable part would drop picked items.
                if (keys.All(key => Guid.TryParse(key, out _)) == false)
                {
                    return settings;
                }

                settings.AllowedTypes = keys
                    .Select(key => GetContentType(configuration.TreeSource?.ObjectType, Guid.Parse(key))?.Alias)
                    .Where(alias => alias != null)
                    .ToArray();
            }

            return settings;
        }

        private IContentTypeComposition GetContentType(string objectType, Guid key)
        {
            switch (objectType)
            {
                case Constants.UdiEntityType.Media:
                    return _mediaTypeService.Get(key);
                case Constants.UdiEntityType.Member:
                    return _memberTypeService.Get(key);
                default:
                    return _contentTypeService.Get(key);
            }
        }
    }
}