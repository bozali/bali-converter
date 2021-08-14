namespace Bali.Converter.Common.Conversion.Video
{
    using System.Threading.Tasks;

    using Bali.Converter.Common.Conversion.Attributes;
    using Bali.Converter.Common.Conversion.Audio;
    using Bali.Converter.Common.Enums;
    using Bali.Converter.FFmpeg;

    [Extension(FileExtensionConstants.Video.Avi)]
    [Target(typeof(Mp4Conversion))]
    [Target(typeof(Mp3Conversion))]
    public class AviConversion : ConversionBase<AviConversion>, IVideoConversion, IAudioConversion
    {
        private readonly IFFmpeg ffmpeg;

        public AviConversion(IFFmpeg ffmpeg)
        {
            this.ffmpeg = ffmpeg;
        }

        public override ConversionTopology Topology
        {
            get => ConversionTopology.Video | ConversionTopology.Audio;
        }

        public VideoConversionOptions VideoConversionOptions { get; set; }

        public AudioConversionOptions AudioConversionOptions { get; set; }

        public override async Task Convert(string source, string destination)
        {
            await this.ffmpeg.Convert(source, destination, new Converter.FFmpeg.VideoConversionOptions
            {
                Filters = this.VideoConversionOptions.VideoFilters
            });
        }
    }
}
