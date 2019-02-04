using Our.Umbraco.SuperValueConverters.Attributes.Core;

namespace Our.Umbraco.SuperValueConverters.Models
{
    public class MNTPSettings : IPickerSettings
    {
        [PreValueProperty("filter")]
        public string[] AllowedDoctypes { get; set; }

        [PreValueProperty("maxNumber")]
        public int MaxItems { get; set; }

        public bool AllowsMultiple()
        {
            return MaxItems == 0 || MaxItems > 1;
        }
    }
}