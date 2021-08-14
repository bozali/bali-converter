namespace Bali.Converter.FFmpeg
{
    using Bali.Converter.FFmpeg.Filters;

    public class VideoConversionOptions
    {
        public int Fps { get; set; }

        //public int From { get; set; }

        //public int To { get; set; }

        public IFilter[] Filters { get; set; }
    }
}
