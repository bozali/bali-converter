namespace Bali.Converter.FFmpeg.Filters.Video
{
    public class RotationFilter : IVideoFilter
    {
        public int Value { get; set; }

        public string GetArgument()
        {
            return $"rotate={this.Value}*PI/180";
        }
    }
}
