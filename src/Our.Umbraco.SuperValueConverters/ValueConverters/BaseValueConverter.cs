using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.SuperValueConverters.Extensions;
using Our.Umbraco.SuperValueConverters.Helpers;
using Our.Umbraco.SuperValueConverters.Models;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.SuperValueConverters.ValueConverters
{
    public class BaseValueConverter
    {
        public static Type GetPropertyValueType(PublishedPropertyType propertyType, IPickerSettings pickerSettings)
        {
            var modelType = typeof(IPublishedContent);

            if (ModelsBuilderHelper.IsEnabled() == true)
            {
                var foundType = GetTypeForPicker(pickerSettings);

                if (foundType != null)
                {
                    modelType = foundType;
                }
            }

            if (pickerSettings.AllowsMultiple() == true)
            {
                return typeof(IEnumerable<>).MakeGenericType(modelType);
            }

            return modelType;
        }

        public static object ConvertIntermediateToObject(PublishedPropertyType propertyType, object source)
        {
            var clrType = propertyType.ClrType;

            bool allowsMultiple = TypeHelper.IsIEnumerable(clrType);

            var innerType = allowsMultiple ? TypeHelper.GetInnerType(clrType) : clrType;

            var list = innerType == typeof(IPublishedContent)
                ? new List<IPublishedContent>()
                : TypeHelper.CreateListOfType(innerType);

            var items = GetItemsFromSource(source);

            foreach (var item in items)
            {
                var itemType = item.GetType();

                if (itemType != innerType)
                {
                    if (innerType == typeof(IPublishedContent)
                        && TypeHelper.IsIPublishedContent(itemType) == true)
                    {
                        list.Add(item);
                    }
                }
                else
                {
                    list.Add(item);
                }
            }

            return allowsMultiple == true ? list : list.FirstOrNull();
        }

        private static Type GetTypeForPicker(IPickerSettings pickerSettings)
        {
            var modelsNamespace = ModelsBuilderHelper.GetNamespace();

            var types = TypeHelper.GetTypes(pickerSettings.AllowedDoctypes, modelsNamespace);

            if (pickerSettings.AllowedDoctypes.Length > 1)
            {
                var interfaces = types.Select(x => x
                        .GetInterfaces()
                        .Where(i => i.IsPublic));

                var sharedInterfaces = interfaces.IntersectMany();

                return sharedInterfaces.LastOrDefault();
            }

            return types.FirstOrDefault();
        }

        private static IEnumerable<IPublishedContent> GetItemsFromSource(object source)
        {
            var sourceItems = new List<IPublishedContent>();

            var sourceAsList = source as IEnumerable<IPublishedContent>;

            if (sourceAsList == null)
            {
                var sourceAsSingle = source as IPublishedContent;

                if (sourceAsSingle != null)
                {
                    sourceItems.Add(sourceAsSingle);
                }
            }
            else
            {
                sourceItems.AddRange(sourceAsList);
            }

            return sourceItems;
        }
    }
}