namespace Bali.Converter.App.Workers
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;

    using Bali.Converter.App.Events;
    using Bali.Converter.App.Modules.Downloads;
    using Bali.Converter.App.Services;
    using Bali.Converter.Common;
    using Bali.Converter.Common.Extensions;
    using Bali.Converter.Common.Media;
    using Bali.Converter.YoutubeDl;

    using log4net;

    using TagLib;
    using TagLib.Id3v2;

    using File = System.IO.File;

    public class DownloadBackgroundWorker
    {
        private readonly IConfigurationService configurationService;
        private readonly IDownloadRegistryService downloadRegistry;
        private readonly IYoutubeDl youtubedl;
        private readonly ILog logger;

        public DownloadBackgroundWorker(IConfigurationService configurationService,
                                        IDownloadRegistryService downloadRegistry,
                                        IYoutubeDl youtubedl)
        {
            this.logger = LogManager.GetLogger(Constants.DownloadServiceLogger);
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
                    job = await this.downloadRegistry.GetDownload();

                    // Skipping empty item. If we have releases but not enough jobs that are not cancelled.
                    if (job == null)
                    {
                        continue;
                    }

                    // If the download is not pending we just skip them, because we did not remove
                    // them from our list.
                    if (job.State != DownloadState.Pending && job.State != DownloadState.Downloading)
                    {
                        continue;
                    }

                    job.State = DownloadState.Downloading;

                    // Create a token to cancel if it was requested while downloading the resource.
                    using var cts = new CancellationTokenSource();

                    // Create a local function to call the cancellation token if cancel is reqeuested.
                    void OnDownloadStateChanged(object s, DownloadStateChangedEventArgs e)
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        if (e.State == DownloadState.Canceled && cts is { IsCancellationRequested: false })
                        {
                            this.logger.Info($"Requesting cancellation for {job.Id}");
                            cts?.Cancel();
                        }
                    }

                    job.DownloadStateChanged += OnDownloadStateChanged;

                    this.logger.Info($"Processing {job.Id} target format {job.TargetFormat}");

                    string downloadPathPattern = Path.Combine(IConfigurationService.TempPath, $"{job.Id:N}.%(ext)s");
                    string downloadPath = downloadPathPattern.Replace("%(ext)s", job.TargetFormat.ToString().ToLowerInvariant());
                    string destinationPath = Path.Combine(this.configurationService.Configuration.DownloadDir, job.Tags.Title + "." + job.TargetFormat.ToString().ToLowerInvariant());

                    // Register a callback that removes the job from the registry and also removes
                    // the part files if cancellation is requested.
                    await using var ctr = cts.Token.Register(() =>
                                                             {
                                                                 job.ProgressText = job.State.ToString("G");

                                                                 var file = new FileInfo(downloadPath + ".part");

                                                                 if (file.WaitForFile())
                                                                 {
                                                                     file.SafeDelete();
                                                                 }
                                                             });

                    this.logger.Debug($"Downloading parameters:");
                    this.logger.Debug($"Pattern Path: {downloadPathPattern}");
                    this.logger.Debug($"Download Path: {downloadPath}");
                    this.logger.Debug($"Destination Path: {destinationPath}");

                    new FileInfo(destinationPath).SafeDelete();

                    var options = new DownloadOptions
                    {
                        DownloadBandwidth = App.IsMinimized ? this.configurationService.Configuration.BandwidthMinimized : this.configurationService.Configuration.Bandwidth,
                        DownloadFormat = job.TargetFormat,
                        Destination = downloadPathPattern,
                    };

                    await this.youtubedl.Download(job.Url, options,
                                                  (f, s) =>
                                                  {
                                                      job.Progress = f;
                                                      job.ProgressText = s;
                                                  },
                                                  cts.Token);

                    cts.Token.ThrowIfCancellationRequested();

                    await this.ApplyTags(downloadPath, job.Tags, job.ThumbnailPath);

                    File.Move(downloadPath, destinationPath);

                    this.downloadRegistry.Complete(job.Id);

                    job.DownloadStateChanged -= OnDownloadStateChanged;
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    this.logger.Error($"Failed downloading", e);
                }
                finally
                {
                    //TODO
                    //if (job != null)
                    //{
                    //    new FileInfo(job.ThumbnailPath).SafeDelete();
                    //}
                }
            }
        }

        private async Task ApplyTags(string path, MediaTags tags, string thumbnailPath)
        {
            this.logger.Info($"Applying tags to {path}");

            using var file = TagLib.File.Create(path);
            file.Tag.Title = tags.Title;
            file.Tag.Album = tags.Album;
            file.Tag.Comment = tags.Comment;
            file.Tag.Copyright = tags.Copyright;
            file.Tag.AlbumArtists = tags.AlbumArtists?.Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.TrimEntries);
            file.Tag.Performers = tags.Performers?.Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.TrimEntries);
            file.Tag.Composers = tags.Composers?.Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.TrimEntries);
            file.Tag.Genres = tags.Genres?.Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (!string.IsNullOrEmpty(thumbnailPath) && new FileInfo(thumbnailPath).Exists)
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
