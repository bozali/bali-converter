namespace Bali.Converter.App.Workers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Bali.Converter.App.Modules.Downloads;
    using Bali.Converter.App.Services;
    using Bali.Converter.Common.Extensions;
    using Bali.Converter.Common.Media;
    using Bali.Converter.YoutubeDl;
    using Bali.Converter.YoutubeDl.Serialization;
    using ImageMagick;

    public class FetchBackgroundWorker
    {
        private readonly IDownloadRegistryService downloadRegistry;
        private readonly IYoutubeDl youtubedl;

        public FetchBackgroundWorker(IDownloadRegistryService downloadRegistry, IYoutubeDl youtubedl)
        {
            this.downloadRegistry = downloadRegistry;
            this.youtubedl = youtubedl;
        }

        public async Task Process(CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var job = await this.downloadRegistry.GetFetch();

                    if (job.State != DownloadState.Fetching)
                    {
                        continue;
                    }

                    // Load information about the url
                    var video = await this.youtubedl.GetVideo(job.Url);

                    job.ThumbnailPath = (await this.DownloadThumbnail(video)).Path;
                    job.Tags = new MediaTags
                    {
                        Title = video.Title.RemoveIllegalChars(),
                        Artist = video.Channel,
                        Year = video.UploadDate.Year
                    };

                    this.downloadRegistry.DownloadFetched(job);
                }
                catch (Exception e)
                {
                }
            }
        }

        private async Task<(string Path, byte[] Data)> DownloadThumbnail(Video video)
        {
            var client = new WebClient();

            byte[] thumbnailData = await client.DownloadDataTaskAsync(video.ThumbnailUrl);

            var thumbnailUri = new Uri(video.ThumbnailUrl);
            string file = thumbnailUri.Segments.Last();

            // ReSharper disable once PossibleNullReferenceException
            string extension = Path.GetExtension(file).Replace(".", string.Empty);

            var destinationThumbnailFile = new FileInfo(Path.Combine(IConfigurationService.TempPath, "Thumbnails", Guid.NewGuid() + ".jpg"));

            if (destinationThumbnailFile.Directory is { Exists: false })
            {
                destinationThumbnailFile.Directory.Create();
            }

            await using var stream = destinationThumbnailFile.OpenWrite();

            // If we have already jpg/jpeg we just write the file and don't recode it.
            if (string.Equals(extension, "jpeg", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(extension, "jpg", StringComparison.OrdinalIgnoreCase))
            {
                await stream.WriteAsync(thumbnailData);
            }
            else
            {
                using var image = new MagickImage(thumbnailData, Enum.Parse<MagickFormat>(extension, true));
                await image.WriteAsync(stream, MagickFormat.Jpeg);
            }

            return (destinationThumbnailFile.FullName, thumbnailData);
        }
    }
}
