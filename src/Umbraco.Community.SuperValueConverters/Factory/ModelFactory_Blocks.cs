using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Community.SuperValueConverters;

public partial class ModelFactory
{
    /// <summary>
    /// Creates a typed instance of <see cref="BlockListItem{TContent, TSettings}"/>
    /// </summary>
    internal object? CreateBlockListItem(Type type, BlockListItem block)
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

        return CreateModel(type,
        [
            (typeof(Guid), block.ContentKey),
            (contentType, block.Content),
            (typeof(Guid?), block.SettingsKey),
            (settingsType, block.Settings)
        ]);
    }
}