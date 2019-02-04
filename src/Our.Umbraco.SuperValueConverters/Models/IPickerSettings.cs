using System;

namespace Our.Umbraco.SuperValueConverters.Models
{
    public interface IPickerSettings
    {
        string[] AllowedDoctypes { get; set; }
        int MaxItems { get; set; }
        bool AllowsMultiple();
    }
}