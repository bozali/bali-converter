namespace Bali.Converter.Common.Conversion.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TargetAttribute : Attribute
    {
        public TargetAttribute(Type target)
        {
            // if (!target.IsSubclassOf(typeof(IConversion)))
            // {
            //     throw new ArgumentException($"{target.FullName} is not a subclass of {nameof(IConversion)}.");
            // }

            this.Target = target;
        }

        public Type Target { get; set; }
    }
}
