namespace Bali.Converter.Common.Conversion
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Bali.Converter.FFmpeg;

    using Unity;

    public class ConversionProvider : IConversionProvider
    {
        private readonly IUnityContainer container;
        private readonly IFFmpeg ffmpeg;

        public ConversionProvider(IUnityContainer container, IFFmpeg ffmpeg)
        {
            this.container = container;
            this.ffmpeg = ffmpeg;
        }

        public string[] GetSupportedFormatsFor(string path)
        {
            string extension = Path.GetExtension(path)?.Replace(".", string.Empty);

            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentException($"Provided path {path} is not a file.");
            }

            var conversionTypes = Assembly.GetAssembly(typeof(IConversion))
                               ?.GetTypes()
                               .Where(t => !t.IsAbstract && t.GetInterfaces().Contains(typeof(IConversion)));

            // ReSharper disable once AssignNullToNotNullAttribute
            var found = conversionTypes.FirstOrDefault(t => string.Equals(ConversionAttributeExtractor.ExtractExtension(t), extension));
            return ConversionAttributeExtractor.ExtractSupportedTypes(found).Select(t => ConversionAttributeExtractor.ExtractExtension(t).ToUpperInvariant()).ToArray();
        }

        public IConversion ProvideFor(string target)
        {
            // string extension = Path.GetExtension(path)?.Replace(".", string.Empty);
            // 
            // if (string.IsNullOrEmpty(extension))
            // {
            //     throw new ArgumentException($"Provided path {path} is not a file.");
            // }

            var conversionTypes = Assembly.GetAssembly(typeof(IConversion))
                                          ?.GetTypes()
                                          .Where(t => !t.IsAbstract && t.GetInterfaces().Contains(typeof(IConversion)))
                                          .ToArray();

            if (conversionTypes == null)
            {
                throw new Exception($"Could not find conversion for {target}.");
            }

            var found = conversionTypes.FirstOrDefault(t => string.Equals(ConversionAttributeExtractor.ExtractExtension(t), target, StringComparison.InvariantCultureIgnoreCase));

            if (found == null)
            {
                throw new Exception($"Could not find conversion for {target}.");
            }

            return this.container.Resolve(found) as IConversion;
        }
    }
}
