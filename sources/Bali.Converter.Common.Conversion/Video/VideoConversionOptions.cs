namespace Bali.Converter.Common.Conversion.Video
{
    using Bali.Converter.FFmpeg.Filters.Video;

    public class VideoConversionOptions
    {
        public IVideoFilter[] VideoFilters { get; set; }

        /// <summary>
        /// The lower the number the better the quality.
        /// </summary>
        public int Quality { get; set; }
    }
}
