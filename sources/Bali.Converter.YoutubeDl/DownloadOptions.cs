namespace Bali.Converter.YoutubeDl
{
    using Bali.Converter.Common.Enums;
    using Bali.Converter.YoutubeDl.Quality;

    public class DownloadOptions
    {
        public DownloadOptions()
        {
            this.DownloadBandwidth = -1;
            this.DownloadFormat = FileExtension.MP4;

            this.Quality = new QualityOption();
        }

        public int DownloadBandwidth { get; set; }

        public string Destination { get; set; }

        public FileExtension DownloadFormat { get; set; }

        public QualityOption Quality { get; set; }
    }
}
