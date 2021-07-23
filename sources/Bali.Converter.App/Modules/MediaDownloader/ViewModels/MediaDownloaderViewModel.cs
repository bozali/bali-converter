namespace Bali.Converter.App.Modules.MediaDownloader.ViewModels
{
    using System;
    using System.IO;
    using System.Linq;
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
    using MahApps.Metro.Controls.Dialogs;
    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    public class MediaDownloaderViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IDownloadRegistry downloadRegistry;
        private readonly IDialogCoordinator dialog;
        private readonly IYoutubeDl youtubedl;

        private string url;
        private string format;

        public MediaDownloaderViewModel(IRegionManager regionManager,
                                        IDownloadRegistry downloadRegistry,
                                        IDialogCoordinator dialog,
                                        IYoutubeDl youtubedl)
        {
            this.regionManager = regionManager;
            this.downloadRegistry = downloadRegistry;
            this.dialog = dialog;
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
            var controller = await this.dialog.ShowProgressAsync(this, "Progress", "Please wait a second...");

            try
            {
                var video = await this.youtubedl.GetVideo(this.Url);

                var job = new DownloadJob
                {
                    ThumbnailPath = await this.DownloadThumbnail(video),
                    TargetFormat = Enum.Parse<MediaFormat>(this.Format, true),
                    Url = this.Url,
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
                await controller.CloseAsync();
            }
        }

        private async Task Edit()
        {
            var controller = await this.dialog.ShowProgressAsync(this, "Progress", "Please wait a second...");

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
                // TODO Catch error and log them
            }
            finally
            {
                await controller.CloseAsync();
            }
        }

        private async Task<string> DownloadThumbnail(Video video)
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

            return destinationThumbnailFile.FullName;
        }
    }
}
