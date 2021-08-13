namespace Bali.Converter.Common.Conversion.Image
{
    using Bali.Converter.FFmpeg.Filters.Video;

    public class ImageConversionOptions
    {
        public IVideoFilter[] VideoFilters { get; set; }
    }
}
