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

            container.Register<Mp3Conversion>();
            container.Register<Mp4Conversion>();
            container.Register<GifConversion>();
            container.Register<AviConversion>();
        }
    }
}
