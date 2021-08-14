namespace Bali.Converter.Common.Conversion.Audio
{
    using Bali.Converter.FFmpeg.Filters;
    using Bali.Converter.FFmpeg.Filters.Audio;

    public class AudioConversionOptions
    {
        public IAudioFilter[] AudioFilters { get; set; }
    }
}
