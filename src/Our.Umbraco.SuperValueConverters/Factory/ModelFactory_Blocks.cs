using System;
using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Our.Umbraco.SuperValueConverters
{
    public partial class ModelFactory
    {
        /// <summary>
        /// Creates a typed instance of <see cref="BlockListItem{TContent, TSettings}"/>
        /// </summary>
        internal object CreateBlockListItem(Type type, BlockListItem block)
        {
            var typeArguments = type.GetGenericArguments();

            var contentType = typeArguments.ElementAtOrDefault(0) ?? typeof(IPublishedElement);

            if (contentType.IsInstanceOfType(block.Content) == false)
            {
                return null;
            }

            var settingsType = typeArguments.ElementAtOrDefault(1) ?? typeof(IPublishedElement);

            if (block.Settings != null && !settingsType.IsInstanceOfType(block.Settings))
            {
                return null;
            }

            return CreateModel(type, new (Type, object)[]
            {
                (typeof(Udi), block.ContentUdi),
                (contentType, block.Content),
                (typeof(Udi), block.SettingsUdi),
                (settingsType, block.Settings)
            });
        }
    }
}