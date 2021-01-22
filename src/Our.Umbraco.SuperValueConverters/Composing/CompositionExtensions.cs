using System;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.SuperValueConverters.Composing
{
    public static class CompositionExtensions
    {
        /// <summary>
        /// De-registers a Property Value Converter while ensuring it remains in the DI container
        /// </summary>
        public static Composition DisableConverter(this Composition composition, Type converter)
        {
            composition.PropertyValueConverters().Remove(converter);

            composition.Register(converter);

            return composition;
        }

        /// <summary>
        /// De-registers a Property Value Converter while ensuring it remains in the DI container
        /// </summary>
        public static Composition DisableConverter<T>(this Composition composition)
            where T : class, IPropertyValueConverter
        {
            return composition.DisableConverter(typeof(T));
        }
    }
}