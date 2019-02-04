using System;

namespace Our.Umbraco.SuperValueConverters.Models
{
    public interface IPickerSettings
    {
        string[] AllowedDoctypes { get; set; }
        int MaxItems { get; set; }
        bool AllowsMultiple();
    }

    public class PickerSettings : IPickerSettings
    {
        public string[] AllowedDoctypes { get; set; }

        public int MaxItems { get; set; }

        public bool AllowsMultiple()
        {
            return MaxItems == 0 || MaxItems > 1;
        }
    }
}