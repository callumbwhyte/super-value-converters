using System.Linq;
using Newtonsoft.Json.Linq;
using Our.Umbraco.SuperValueConverters.Attributes.Core;

namespace Our.Umbraco.SuperValueConverters.Attributes
{
    public class NestedContentAllowedDoctypesFilterAttribute : PreValueFilterAttribute
    {
        public override object Process(string input)
        {
            var contentTypesJson = JArray.Parse(input);

            if (contentTypesJson != null)
            {
                var allowedDoctypes = contentTypesJson
                    .Select(x => x.Value<string>("ncAlias"))
                    .ToArray();

                return allowedDoctypes;
            }

            return new string[] { };
        }
    }
}