using System;

namespace Reporting.Application.Pipeline.ValidatorPipeline.Attribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAutomaticValidationAttribute : System.Attribute
    {
    }
}