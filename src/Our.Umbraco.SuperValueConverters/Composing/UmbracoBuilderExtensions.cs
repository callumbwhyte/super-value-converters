using System;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.PropertyEditors;

namespace Our.Umbraco.SuperValueConverters.Composing
{
    public static class UmbracoBuilderExtensions
    {
        /// <summary>
        /// De-registers a Property Value Converter while ensuring it remains in the DI container
        /// </summary>
        public static void DisableConverter(this IUmbracoBuilder builder, Type converter)
        {
            builder.PropertyValueConverters().Remove(converter);

            builder.Services.AddTransient(converter);
        }

        /// <summary>
        /// De-registers a Property Value Converter while ensuring it remains in the DI container
        /// </summary>
        public static void DisableConverter<T>(this IUmbracoBuilder builder)
            where T : class, IPropertyValueConverter
        {
            builder.DisableConverter(typeof(T));
        }
    }
}