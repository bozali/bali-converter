namespace Bali.Converter.Common.Conversion
{
    using Bali.Converter.Common.Conversion.Audio;
    using Bali.Converter.Common.Conversion.Image;
    using Bali.Converter.Common.Conversion.Video;

    using Prism.Ioc;

    public static class DependencyRegistration
    {
        public static void RegisterConversions(this IContainerRegistry container)
        {
            container.RegisterSingleton<IConversionProvider, ConversionProvider>();

            // Audio formats
            container.Register<Mp3Conversion>();
            container.Register<WavConversion>();

            // Video formats
            container.Register<Mp4Conversion>();
            container.Register<AviConversion>();

            // Image formats
            container.Register<GifConversion>();
        }
    }
}
