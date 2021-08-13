namespace Bali.Converter.FFmpeg.Filters.Audio
{
    public class VolumeFilter : IAudioFilter
    {
        public float MultiplierValue { get; set; }

        public string GetArgument()
        {
            return $"volume={this.MultiplierValue}";
        }
    }
}
