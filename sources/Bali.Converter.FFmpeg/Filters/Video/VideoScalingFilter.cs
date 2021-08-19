namespace Bali.Converter.FFmpeg.Filters.Video
{
    public class VideoScalingFilter : IVideoFilter
    {


        public string GetArgument()
        {
            return @"scale=";
        }
    }
}
