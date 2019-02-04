using Our.Umbraco.SuperValueConverters.Attributes;
using Our.Umbraco.SuperValueConverters.Attributes.Core;

namespace Our.Umbraco.SuperValueConverters.Models
{
    public class NestedContentSettings : IPickerSettings
    {
        [PreValueProperty("contentTypes")]
        [NestedContentAllowedDoctypesFilter]
        public string[] AllowedDoctypes { get; set; }

        [PreValueProperty("maxItems")]
        public int MaxItems { get; set; }

        public bool AllowsMultiple()
        {
            return MaxItems == 0 || MaxItems > 1;
        }
    }
}