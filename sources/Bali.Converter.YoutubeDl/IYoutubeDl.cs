namespace Bali.Converter.YoutubeDl
{
    using System;
    using System.Threading.Tasks;
    using Bali.Converter.Common.Enums;
    using Bali.Converter.YoutubeDl.Models;

    public interface IYoutubeDl
    {
        Task<Video> GetVideo(string url);

        Task Download(string url, string path, MediaFormat format, Action<float, string> progressReport = null);
    }
}
