using System.Collections.Generic;

namespace Our.Umbraco.SuperValueConverters.Attributes
{
    public class MediaPickerAllowedDoctypesFilterAttribute : PreValueFilterAttribute
    {
        public override object Process(string input)
        {
            var value = new List<string>();

            if (input == "1")
            {
                value.Add("Image");
            }

            return value.ToArray();
        }
    }
}