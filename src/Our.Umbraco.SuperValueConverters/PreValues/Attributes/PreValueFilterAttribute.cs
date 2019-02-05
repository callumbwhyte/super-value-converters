using System;

namespace Our.Umbraco.SuperValueConverters.PreValues.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public abstract class PreValueFilterAttribute : Attribute
    {
        public virtual object Process(string input)
        {
            return input;
        }
    }
}