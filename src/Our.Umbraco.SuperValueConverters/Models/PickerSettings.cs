using System;

namespace Our.Umbraco.SuperValueConverters.Models
{
    internal class PickerSettings : IPickerSettings
    {
        public string[] AllowedTypes { get; set; } = new string[] { };

        public int MaxItems { get; set; }

        public Type DefaultType { get; set; }

        public bool AllowsMultiple()
        {
            return MaxItems == 0 || MaxItems > 1;
        }
    }
}