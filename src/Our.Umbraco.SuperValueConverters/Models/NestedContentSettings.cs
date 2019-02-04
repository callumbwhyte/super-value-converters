using Our.Umbraco.SuperValueConverters.Attributes;

namespace Our.Umbraco.SuperValueConverters.Models
{
    public class NestedContentSettings : IPickerSettings
    {
        [PreValueProperty("contentTypes")]
        public string[] AllowedDoctypes { get; set; }

        [PreValueProperty("maxItems")]
        public int MaxItems { get; set; }

        public bool AllowsMultiple()
        {
            return MaxItems == 0 || MaxItems > 1;
        }
    }
}