namespace Bali.Converter.YoutubeDl.Quality
{
    using Bali.Converter.YoutubeDl.Serialization;

    public class QualityOption
    {
        public QualityOption()
        {
            this.VideoQuality = AutomaticQualityOption.Manual;
            this.AudioQuality = AutomaticQualityOption.Manual;
        }

        public VideoFormat SelectedVideoFormat { get; set; }

        public AutomaticQualityOption AudioQuality { get; set; }

        public AutomaticQualityOption VideoQuality { get; set; }
    }
}
