using System.Linq;

namespace Our.Umbraco.SuperValueConverters.Models
{
    public class PickerSettings
    {
        public string[] AllowedDoctypes { get; set; }

        public int MaxItems { get; set; }

        public bool AllowsMultiple()
        {
            return AllowedDoctypes.Count() > 1 || MaxItems > 1;
        }
    }
}