namespace Bali.Converter.Common.Conversion.Audio
{
    using System.Threading.Tasks;

    using Bali.Converter.Common.Conversion.Attributes;
    using Bali.Converter.Common.Enums;
    using Bali.Converter.FFmpeg;

    [Extension(FileExtensionConstants.Audio.Wav)]
    [Target(typeof(WavConversion))]
    [Target(typeof(Mp3Conversion))]
    public class WavConversion : ConversionBase<WavConversion>, IAudioConversion
    {
        private readonly IFFmpeg ffmpeg;

        public WavConversion(IFFmpeg ffmpeg)
        {
            this.ffmpeg = ffmpeg;
        }

        public override ConversionTopology Topology
        {
            get => ConversionTopology.Audio;
        }

        public AudioConversionOptions AudioConversionOptions { get; set; }

        public override async Task Convert(string source, string destination)
        {
            await this.ffmpeg.Convert(source, destination, new ConversionOptions
            {
                Filters = this.AudioConversionOptions.AudioFilters
            });
        }
    }
}
