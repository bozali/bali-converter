namespace Bali.Converter.Common.Conversion
{
    using System;
    using System.Linq;

    using Bali.Converter.Common.Conversion.Attributes;

    public static class ConversionAttributeExtractor
    {
        public static string ExtractExtension(Type t)
        {
            if (!t.GetInterfaces().Contains(typeof(IConversion)))
            {
                throw new ArgumentException(nameof(t));
            }

            if (Attribute.GetCustomAttribute(t, typeof(ExtensionAttribute)) is not ExtensionAttribute extensionAttribute)
            {
                throw new Exception($"Extension is missing for {t.FullName}.");
            }

            return extensionAttribute.Extension;
        }

        public static Type[] ExtractSupportedTypes(Type t)
        {
            var targetAttributes = Attribute.GetCustomAttributes(t, typeof(TargetAttribute)) as TargetAttribute[];

            if (targetAttributes == null || !targetAttributes.Any())
            {
                throw new ArgumentException($"Target is missing for {t.FullName}.");
            }

            return targetAttributes.Select(t => t.Target).ToArray();
        }
    }
}
