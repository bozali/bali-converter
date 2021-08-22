namespace Bali.Converter.Common.Conversion.Video
{
    using System.Threading.Tasks;

    using Bali.Converter.Common.Conversion.Attributes;
    using Bali.Converter.Common.Conversion.Audio;
    using Bali.Converter.Common.Enums;

    [Extension(FileExtensionConstants.Video.Mkv)]
    [Target(typeof(AviConversion))]
    [Target(typeof(Mp4Conversion))]
    public class MkvConversion : ConversionBase<MkvConversion>, IVideoConversion, IAudioConversion
    {
        public override ConversionTopology Topology
        {
            get => ConversionTopology.Audio | ConversionTopology.Video;
        }

        public VideoConversionOptions VideoConversionOptions { get; set; }

        public AudioConversionOptions AudioConversionOptions { get; set; }

        public override Task Convert(string source, string destination)
        {
            return Task.CompletedTask;
        }
    }
}
