using System;

namespace Our.Umbraco.SuperValueConverters.Models
{
    [Obsolete]
    public interface IPickerSettings
    {
        string[] AllowedTypes { get; set; }

        int MaxItems { get; set; }

        Type BaseType { get; set; }

        bool AllowsMultiple();
    }
}