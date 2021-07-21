namespace Bali.Converter.App.Modules.MediaDownloader.ViewModels
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    using Bali.Converter.App.Modules.Downloads;
    using Bali.Converter.App.Modules.MediaDownloader.Views;
    using Bali.Converter.App.Services;
    using Bali.Converter.Common.Enums;
    using Bali.Converter.Common.Extensions;
    using Bali.Converter.Common.Media;
    using Bali.Converter.YoutubeDl;
    using Bali.Converter.YoutubeDl.Models;
    using ImageMagick;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    public class MediaDownloaderViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IDownloadRegistry downloadRegistry;
        private readonly IYoutubeDl youtubedl;

        private string url;
        private string format;

        public MediaDownloaderViewModel(IRegionManager regionManager,
                                        IDownloadRegistry downloadRegistry,
                                        IYoutubeDl youtubedl)
        {
            this.regionManager = regionManager;
            this.downloadRegistry = downloadRegistry;
            this.youtubedl = youtubedl;

            this.DownloadCommand = new DelegateCommand(async () => await this.Download(), () => !string.IsNullOrEmpty(this.Url));
            this.EditCommand = new DelegateCommand(async () => await this.Edit(), () => !string.IsNullOrEmpty(this.Url));

            this.Format = MediaFormat.MP4.ToString();
        }

        public string Url
        {
            get => this.url;
            set
            {
                if (this.SetProperty(ref this.url, value))
                {
                    this.DownloadCommand.RaiseCanExecuteChanged();
                    this.EditCommand.RaiseCanExecuteChanged();
                }
            } 
        }

        public string Format
        {
            get => this.format;
            set => this.SetProperty(ref this.format, value);
        }

        public DelegateCommand DownloadCommand { get; }

        public DelegateCommand EditCommand { get; }
        
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.Url = string.Empty;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private async Task Download()
        {
            try
            {
                var video = await this.youtubedl.GetVideo(this.Url);

                var job = new DownloadJob
                {
                    ThumbnailPath = await this.DownloadThumbnail(video),
                    TargetFormat = Enum.Parse<MediaFormat>(this.Format, true),
                    Tags = new MediaTags
                    {
                        Title = video.Title.RemoveIllegalChars(),
                        Artist = video.Channel,
                        Year = video.UploadDate.Year
                    }
                };

                this.downloadRegistry.Add(job);

                this.regionManager.Regions["ContentRegion"].RequestNavigate(nameof(MediaDownloaderView));
            }
            catch (Exception e)
            {
                // TODO Catch error and log them
            }
            finally
            {
            }
        }

        private async Task Edit()
        {
            try
            {
                var video = await this.youtubedl.GetVideo(this.Url);

                var parameters = new NavigationParameters();
                parameters.Add("ThumbnailPath", await this.DownloadThumbnail(video));
                parameters.Add("Format", this.Format);
                parameters.Add("Video", video);

                this.regionManager.Regions["ContentRegion"].RequestNavigate(nameof(SingleMediaEditorView), parameters);
            }
            catch (Exception e)
            {
            }
        }

        private async Task<string> DownloadThumbnail(Video video)
        {
            var client = new WebClient();
            byte[] thumbnailData = await client.DownloadDataTaskAsync(video.ThumbnailUrl);

            // ReSharper disable once PossibleNullReferenceException
            string extension = Path.GetExtension(video.ThumbnailUrl).Replace(".", string.Empty);
            using var image = new MagickImage(thumbnailData, Enum.Parse<MagickFormat>(extension, true));

            var thumbnailFile = new FileInfo(Path.Combine(IConfigurationService.TempPath, "Thumbnails", Guid.NewGuid() + ".jpg"));

            if (thumbnailFile.Directory is { Exists: false })
            {
                thumbnailFile.Directory.Create();
            }

            await using var stream = thumbnailFile.OpenWrite();
            await image.WriteAsync(stream, MagickFormat.Jpeg);

            return thumbnailFile.FullName;
        }
    }
}
