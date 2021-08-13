namespace Bali.Converter.Common.Conversion.Video
{
    using System.Threading.Tasks;

    using Bali.Converter.Common.Conversion.Attributes;
    using Bali.Converter.Common.Conversion.Audio;
    using Bali.Converter.Common.Enums;

    [Extension(FileExtensionConstants.Video.Avi)]
    [Target(typeof(Mp4Conversion))]
    [Target(typeof(Mp3Conversion))]
    public class AviConversion : ConversionBase<AviConversion>, IVideoConversion, IAudioConversion
    {
        public override ConversionTopology Topology
        {
            get => ConversionTopology.Video | ConversionTopology.Audio;
        }

        public VideoConversionOptions VideoConversionOptions { get; set; }

        public AudioConversionOptions AudioConversionOptions { get; set; }

        public override Task Convert(string path)
        {
            return Task.CompletedTask;
        }
    }
}
