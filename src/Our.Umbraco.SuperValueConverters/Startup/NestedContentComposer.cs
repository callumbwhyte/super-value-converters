using Our.Umbraco.SuperValueConverters.ValueConverters;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web.PropertyEditors.ValueConverters;

namespace Our.Umbraco.SuperValueConverters.Startup
{
    public class NestedContentComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.PropertyValueConverters()
                .Replace<NestedContentManyValueConverter, NestedContentValueConverter>()
                .Replace<NestedContentSingleValueConverter, NestedContentValueConverter>();
        }
    }
}