namespace Bali.Converter.Common.Conversion.Attributes
{
    using System;

    // NOTE For now we support only one extension attribute.
    [AttributeUsage(AttributeTargets.Class)]
    public class ExtensionAttribute : Attribute
    {
        public ExtensionAttribute(string extension)
        {
            this.Extension = extension;
        }

        public string Extension { get; }
    }
}
