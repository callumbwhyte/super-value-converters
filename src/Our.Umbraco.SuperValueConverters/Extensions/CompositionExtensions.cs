using System;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.SuperValueConverters.Extensions
{
    internal static class CompositionExtensions
    {
        public static Composition DisableConverter(this Composition composition, Type converter)
        {
            // de-register property value converter
            composition.PropertyValueConverters().Remove(converter);

            // ensure the converter remains in the DI container
            composition.Register(converter);

            return composition;
        }

        public static Composition DisableConverter<T>(this Composition composition)
            where T : class, IPropertyValueConverter
        {
            return composition.DisableConverter(typeof(T));
        }
    }
}