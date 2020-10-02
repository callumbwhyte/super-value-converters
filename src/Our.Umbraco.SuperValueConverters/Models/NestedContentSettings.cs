using Our.Umbraco.SuperValueConverters.Attributes;
using Our.Umbraco.SuperValueConverters.PreValues.Attributes;

namespace Our.Umbraco.SuperValueConverters.Models
{
    internal class NestedContentSettings : IPickerSettings
    {
        [PreValueProperty("contentTypes")]
        [NestedContentAllowedDoctypesFilter]
        public string[] AllowedDoctypes { get; set; } = new string[] { };

        [PreValueProperty("maxItems")]
        public int MaxItems { get; set; }

        public bool AllowsMultiple()
        {
            return MaxItems == 0 || MaxItems > 1;
        }
    }
}