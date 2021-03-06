namespace Bali.Converter.App.Modules.MediaDownloader.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;

    using Bali.Converter.App.Modules.Downloads;
    using Bali.Converter.App.Modules.MediaDownloader.Views;
    using Bali.Converter.App.Services;
    using Bali.Converter.Common;
    using Bali.Converter.Common.Enums;
    using Bali.Converter.Common.Extensions;
    using Bali.Converter.Common.Media;
    using Bali.Converter.YoutubeDl;
    using Bali.Converter.YoutubeDl.Quality;
    using Bali.Converter.YoutubeDl.Serialization;

    using ImageMagick;

    using log4net;

    using MahApps.Metro.Controls.Dialogs;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    public class MediaDownloaderViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IDownloadRegistryService downloadRegistry;
        private readonly IDialogCoordinator dialog;
        private readonly IYoutubeDl youtubedl;
        private readonly ILog logger;

        private AutomaticQualityOption audioQualityOption;
        private AutomaticQualityOption videoQualityOption;

        private string url;
        private string format;
        private bool proceedAsPlaylist;

        public MediaDownloaderViewModel(IRegionManager regionManager,
                                        IDownloadRegistryService downloadRegistry,
                                        IDialogCoordinator dialog,
                                        IYoutubeDl youtubedl)
        {
            this.logger = LogManager.GetLogger(Constants.ApplicationLogger);

            this.regionManager = regionManager;
            this.downloadRegistry = downloadRegistry;
            this.dialog = dialog;
            this.youtubedl = youtubedl;

            this.DownloadCommand = new DelegateCommand(async () => await this.Download(), () => !string.IsNullOrEmpty(this.Url));
            this.EditCommand = new DelegateCommand(async () => await this.Edit(), () => !string.IsNullOrEmpty(this.Url));

            this.Format = FileExtension.MP4.ToString();
        }

        public AutomaticQualityOption AudioQualityOption
        {
            get => this.audioQualityOption;
            set => this.SetProperty(ref this.audioQualityOption, value);
        }

        public AutomaticQualityOption VideoQualityOption
        {
            get => this.videoQualityOption;
            set => this.SetProperty(ref this.videoQualityOption, value);
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

                    this.RaisePropertyChanged(nameof(this.IsPlaylist));
                }
            } 
        }

        public string Format
        {
            get => this.format;
            set => this.SetProperty(ref this.format, value);
        }

        public bool ProceedAsPlaylist
        {
            get => this.proceedAsPlaylist;
            set => this.SetProperty(ref this.proceedAsPlaylist, value);
        }

        public bool IsPlaylist
        {
            get
            {
                if (string.IsNullOrEmpty(this.Url))
                {
                    return false;
                }

                if (!Uri.TryCreate(this.Url, UriKind.Absolute, out var uri))
                {
                    return false;
                }

                return HttpUtility.ParseQueryString(uri.Query)
                                  .AllKeys
                                  .Any(key => string.Equals(key, "list", StringComparison.OrdinalIgnoreCase));
            }
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
                if (this.IsPlaylist && this.ProceedAsPlaylist)
                {
                    // TODO We need to add functionality to fetch playlist
                    var videos = await this.youtubedl.GetVideos(this.Url);
                    
                    foreach (var video in videos)
                    {
                        await this.RegisterJob(video);
                    }
                }
                else
                {
                    this.downloadRegistry.AddFetch(this.Url, Enum.Parse<FileExtension>(this.Format, true));

                    // var video = await this.youtubedl.GetVideo(this.Url);
                    // await this.RegisterJob(video);
                }
                
                this.regionManager.RequestNavigate("ContentRegion", nameof(MediaDownloaderView));
            }
            catch (Exception e)
            {
                this.logger.Error($"{nameof(MediaDownloaderViewModel)} had an error while downloading.", e);
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
                if (this.IsPlaylist && this.ProceedAsPlaylist)
                {
                    var resources = await this.youtubedl.GetVideos(this.Url);
                    var videos = new List<VideoViewModel>();

                    foreach (var resource in resources)
                    {
                        videos.Add(await this.ConvertVideoResource(resource));
                    }

                    var parameters = new NavigationParameters();
                    parameters.Add("Videos", videos);

                    this.regionManager.RequestNavigate("ContentRegion", nameof(PlaylistSelectionView), parameters);
                }
                else
                {
                    var resource = await this.youtubedl.GetVideo(this.Url);
                    var video = await this.ConvertVideoResource(resource);

                    var parameters = new NavigationParameters();
                    parameters.Add("Video", video);
                    parameters.Add("VideoQuality", this.VideoQualityOption);
                    parameters.Add("AudioQuality", this.AudioQualityOption);

                    this.regionManager.RequestNavigate("ContentRegion", nameof(SingleMediaEditorView), parameters);
                }
            }
            catch (Exception e)
            {
                this.logger.Error($"{nameof(MediaDownloaderViewModel)} had an error while editing.", e);
            }
            finally
            {
                await controller.CloseAsync();
            }
        }

        private async Task RegisterJob(Video video)
        {
            var job = new DownloadJob
            {
                ThumbnailPath = (await this.DownloadThumbnail(video)).Path,
                TargetFormat = Enum.Parse<FileExtension>(this.Format, true),
                Url = this.Url,
                Tags = new MediaTags
                {
                    Title = video.Title.RemoveIllegalChars(),
                    Artist = video.Channel,
                    Year = video.UploadDate.Year
                }
            };

            this.downloadRegistry.AddDownload(job);
        }

        private async Task<VideoViewModel> ConvertVideoResource(Video video)
        {
            (string path, byte[] data) = await this.DownloadThumbnail(video);

            return new VideoViewModel
            {
                ThumbnailPath = path,
                ThumbnailData = data,
                Format = this.Format,
                Url = video.Url,
                Tags = new MediaTagsViewModel
                {
                    Title = video.Title.RemoveIllegalChars(),
                    Artist = video.Channel,
                    Year = video.UploadDate.Year
                }
            };
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
