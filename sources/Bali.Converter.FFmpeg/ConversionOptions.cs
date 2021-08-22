namespace Bali.Converter.FFmpeg
{
    using System;

    using Bali.Converter.FFmpeg.Filters;

    public class ConversionOptions
    {
        public int Fps { get; set; }

        public TimeSpan? StartPosition { get; set; }

        public TimeSpan? EndPosition { get; set; }

        public IFilter[] Filters { get; set; }
    }
}
