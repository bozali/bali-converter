namespace Bali.Converter.YoutubeDl
{
    using Bali.Converter.Common.Enums;

    public class DownloadOptions
    {
        public int DownloadBandwidth { get; set; }

        public string Destination { get; set; }

        public FileExtension DownloadFormat { get; set; }
    }
}
