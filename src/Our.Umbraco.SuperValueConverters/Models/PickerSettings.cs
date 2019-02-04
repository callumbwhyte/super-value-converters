using System.Linq;

namespace Our.Umbraco.SuperValueConverters.Models
{
    public class PickerSettings
    {
        public string[] AllowedDoctypes { get; set; }

        public int MaxItems { get; set; }

        public bool AllowsMultiple()
        {
            return MaxItems == 0 || MaxItems > 1;
        }
    }
}