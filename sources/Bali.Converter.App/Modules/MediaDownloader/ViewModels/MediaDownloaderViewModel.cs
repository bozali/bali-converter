namespace Bali.Converter.App.Modules.MediaDownloader.ViewModels
{
    using System;

    using Bali.Converter.App.Modules.Downloads;
    using Bali.Converter.App.Modules.MediaDownloader.Views;
    using Bali.Converter.Common.Enums;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    public class MediaDownloaderViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IDownloadRegistry downloadRegistry;

        private bool isProcessing;
        private string url;
        private string format;

        public MediaDownloaderViewModel(IRegionManager regionManager, IDownloadRegistry downloadRegistry)
        {
            this.regionManager = regionManager;
            this.downloadRegistry = downloadRegistry;

            this.DownloadCommand = new DelegateCommand(this.Download, () => !string.IsNullOrEmpty(this.Url));

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

        private void Download()
        {
            try
            {
                this.IsProcessing = true;

                this.downloadRegistry.Add(this.Url, Enum.Parse<MediaFormat>(this.Format, true));

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
