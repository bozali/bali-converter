namespace Bali.Converter.FFmpeg.Filters.Video
{
    public class RotationFilter : IVideoFilter
    {
        public int Rotation { get; set; }

        public string GetArgument()
        {
            return $"rotate={this.Rotation}*PI/180";
        }
    }
}
