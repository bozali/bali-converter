namespace Bali.Converter.App.Modules.Conversion
{
    using System;
    using System.IO;

    using Bali.Converter.Common.Enums;

    public static class ConversionMetadataFactory
    {
        public static ConversionMetadata CreateMetadata(string path)
        {
            string extension = Path.GetExtension(path)?.Replace(".", string.Empty);

            if (!Enum.TryParse(extension, true, out DocumentExtension documentExtension))
            {
                return null;
            }

            return new ConversionMetadata
            {
                Extension = documentExtension,
                SourcePath = path
            };
        }
    }
}
