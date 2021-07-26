namespace Bali.Converter.App.Workers
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using Bali.Converter.App.Modules.Downloads;
    using Bali.Converter.App.Services;
    using Bali.Converter.Common.Extensions;
    using Bali.Converter.Common.Media;
    using Bali.Converter.YoutubeDl;
    using TagLib;
    using TagLib.Id3v2;
    using File = System.IO.File;

    public class DownloadBackgroundWorker
    {
        private readonly IConfigurationService configurationService;
        private readonly IDownloadRegistry downloadRegistry;
        private readonly IYoutubeDl youtubedl;

        public DownloadBackgroundWorker(IConfigurationService configurationService,
                                        IDownloadRegistry downloadRegistry,
                                        IYoutubeDl youtubedl)
        {
            this.configurationService = configurationService;
            this.downloadRegistry = downloadRegistry;
            this.youtubedl = youtubedl;
        }

        public async Task Process(CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                DownloadJob job = null;

                try
                {
                    job = await this.downloadRegistry.Get();

                    string downloadPathPattern = Path.Combine(IConfigurationService.TempPath, $"{job.Id:N}.%(ext)s");
                    string downloadPath = downloadPathPattern.Replace("%(ext)s", job.TargetFormat.ToString().ToLowerInvariant());
                    string destinationPath = Path.Combine(this.configurationService.Configuration.DownloadDir, job.Tags.Title + "." + job.TargetFormat.ToString().ToLowerInvariant());

                    await this.youtubedl.Download(job.Url, downloadPathPattern, job.TargetFormat,
                                                  (f, s) =>
                                                  {
                                                      job.Progress = f;
                                                      job.ProgressText = s;
                                                  });

                    await this.ApplyTags(downloadPath, job.Tags, job.ThumbnailPath);

                    File.Move(downloadPath, destinationPath);

                    this.downloadRegistry.Remove(job.Id);
                }
                catch (Exception e)
                {
                    // TODO
                }
            }
        }

        private async Task ApplyTags(string path, MediaTags tags, string thumbnailPath)
        {
            using var file = TagLib.File.Create(path);
            file.Tag.Title = tags.Title;
            file.Tag.Album = tags.Album;
            file.Tag.Comment = tags.Comment;
            file.Tag.Copyright = tags.Copyright;
            file.Tag.AlbumArtists = tags.AlbumArtists?.Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.TrimEntries);
            file.Tag.Performers = tags.Performers?.Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.TrimEntries);
            file.Tag.Composers = tags.Composers?.Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.TrimEntries);
            file.Tag.Genres = tags.Genres?.Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (!string.IsNullOrEmpty(thumbnailPath))
            {
                byte[] data = await File.ReadAllBytesAsync(thumbnailPath);

                file.Tag.Pictures = new IPicture[]
                {
                    new AttachedPictureFrame
                    {
                        Data = new ByteVector(data),
                        MimeType = MediaTypeNames.Image.Jpeg,
                        Type = PictureType.FrontCover,
                        TextEncoding = StringType.UTF16,
                    }
                };
            }

            file.Save();
        }
    }
}
