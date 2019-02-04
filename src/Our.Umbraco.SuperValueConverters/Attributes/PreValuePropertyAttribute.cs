using System;

namespace Our.Umbraco.SuperValueConverters.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PreValuePropertyAttribute : Attribute
    {
        public PreValuePropertyAttribute(string alias)
        {
            Alias = alias;
        }

        public string Alias { get; set; }
    }
}