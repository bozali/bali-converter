namespace Bali.Converter.FFmpeg.Filters.Video
{
    public class FpsFilter : IVideoFilter
    {
        public int Fps { get; set; }

        public string GetArgument()
        {
            return $"fps={this.Fps}";
        }
    }
}
