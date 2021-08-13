namespace Bali.Converter.FFmpeg.Filters.Video
{
    public class VideoScaling : IVideoFilter
    {
        public string GetArgument()
        {
            return @"scale=";
        }
    }
}
