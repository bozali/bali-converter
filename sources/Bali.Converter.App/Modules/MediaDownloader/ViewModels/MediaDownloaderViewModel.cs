namespace Bali.Converter.App.Modules.MediaDownloader.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using Bali.Converter.App.Modules.Downloads;
    using Bali.Converter.App.Modules.MediaDownloader.Views;
    using Bali.Converter.Common.Enums;
    using Bali.Converter.Common.Extensions;
    using Bali.Converter.Common.Media;
    using Bali.Converter.YoutubeDl;
    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    public class MediaDownloaderViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IDownloadRegistry downloadRegistry;
        private readonly IYoutubeDl youtubedl;

        private bool isProcessing;
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
                }
            } 
        }

        public bool IsProcessing
        {
            get => this.isProcessing;
            set => this.SetProperty(ref this.isProcessing, value);
        }

        public string Format
        {
            get => this.format;
            set => this.SetProperty(ref this.format, value);
        }

        public DelegateCommand DownloadCommand { get; }
        
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
                this.IsProcessing = true;

                var video = await this.youtubedl.GetVideo(this.Url);

                this.downloadRegistry.Add(this.Url, Enum.Parse<MediaFormat>(this.Format, true), new MediaTags
                {
                    Title = video.Title.RemoveIllegalChars(),
                    Artist = video.Channel
                });

                this.regionManager.Regions["ContentRegion"].RequestNavigate(nameof(MediaDownloaderView));
            }
            catch (Exception e)
            {
                // TODO Catch error and log them
            }
            finally
            {
                this.IsProcessing = false;
            }
        }
    }
}
