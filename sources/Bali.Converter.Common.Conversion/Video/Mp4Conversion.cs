namespace Bali.Converter.Common.Conversion.Video
{
    using System.Threading.Tasks;

    using Bali.Converter.Common.Conversion.Attributes;
    using Bali.Converter.Common.Conversion.Audio;
    using Bali.Converter.Common.Enums;
    using Bali.Converter.FFmpeg;

    [Extension(FileExtensionConstants.Video.Mp4)]
    [Target(typeof(Mp3Conversion))]
    [Target(typeof(AviConversion))]
    public class Mp4Conversion : ConversionBase<Mp4Conversion>, IVideoConversion, IAudioConversion
    {
        private readonly IFFmpeg ffmpeg;

        public Mp4Conversion(IFFmpeg ffmpeg)
        {
            this.ffmpeg = ffmpeg;
        }

        public override ConversionTopology Topology
        {
            get => ConversionTopology.Video | ConversionTopology.Audio;
        }

        public VideoConversionOptions VideoConversionOptions { get; set; }

        public AudioConversionOptions AudioConversionOptions { get; set; }

        public override Task Convert(string source, string destination)
        {
            return Task.CompletedTask;
        }
    }
}
