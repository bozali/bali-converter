namespace Bali.Converter.Common.Conversion.Image
{
    using System.Threading.Tasks;

    using Bali.Converter.Common.Conversion.Attributes;
    using Bali.Converter.Common.Conversion.Video;
    using Bali.Converter.Common.Enums;

    [Extension("gif")]
    [Target(typeof(Mp4Conversion))]
    public class GifConversion : ConversionBase<GifConversion>, IVideoConversion, IImageConversion
    {
        public override ConversionTopology Topology
        {
            get => ConversionTopology.Image | ConversionTopology.Video;
        }

        public ImageConversionOptions ImageConversionOptions { get; set; }

        public VideoConversionOptions VideoConversionOptions { get; set; }

        public override Task Convert(string source, string destination)
        {
            return Task.CompletedTask;
        }
    }
}
