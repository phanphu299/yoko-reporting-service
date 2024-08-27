using System;

namespace Reporting.Application.Pipeline.ValidatorPipeline.Attribute
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RemoteValidatorKeyAttribute : System.Attribute
    {
        public string Key { get; private set; }

        public RemoteValidatorKeyAttribute(string key)
        {
            Key = key;
        }
    }
}