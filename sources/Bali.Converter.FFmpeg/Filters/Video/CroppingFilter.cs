namespace Bali.Converter.FFmpeg.Filters.Video
{
    using System.Drawing;

    public class CroppingFilter : IVideoFilter
    {
        public Size Size { get; set; }

        /// <summary>
        /// If this is not specified it will be centered.
        /// </summary>
        public Point? Position { get; set; }

        public string GetArgument()
        {
            return @$"crop=w=";
        }
    }
}
