using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;

namespace Our.Umbraco.SuperValueConverters.Helpers
{
    public class DataTypeHelper
    {
        public static IDictionary<string, string> GetPreValues(int dataTypeId)
        {
            var dataTypeService = ApplicationContext.Current.Services.DataTypeService;

            var preValues = dataTypeService.GetPreValuesCollectionByDataTypeId(dataTypeId);

            var preValuesDictionary = preValues.PreValuesAsDictionary.ToDictionary(x => x.Key, x => x.Value.Value);

            return preValuesDictionary;
        }
    }
}