using System;

namespace Our.Umbraco.SuperValueConverters.Models
{
    public class NestedContentSettings : IPickerSettings
    {
        public string[] AllowedDoctypes { get; set; }

        public int MaxItems { get; set; }

        public bool AllowsMultiple()
        {
            return MaxItems == 0 || MaxItems > 1;
        }
    }
}