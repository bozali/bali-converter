namespace Bali.Converter.YoutubeDl
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Bali.Converter.YoutubeDl.Models;

    public interface IYoutubeDl
    {
        Task<IReadOnlyCollection<Video>> GetVideos(string url);

        Task<Video> GetVideo(string url);

        Task Download(string url, DownloadOptions options, Action<float, string> progressReport = null, CancellationToken ct = default);
    }
}
