using System;

namespace Our.Umbraco.SuperValueConverters.Attributes
{
    public class MediaPickerMaxItemsFilterAttribute : PreValueFilterAttribute
    {
        public override object Process(string input)
        {
            int value = 0;

            if (input != "1")
            {
                value = 1;
            }

            return value;
        }
    }
}