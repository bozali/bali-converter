namespace Bali.Converter.App.Modules.MediaDownloader.ViewModels
{
    using System;
    using System.IO;

    using AutoMapper;

    using Bali.Converter.App.Modules.Downloads;
    using Bali.Converter.App.Modules.MediaDownloader.Views;
    using Bali.Converter.Common.Enums;
    using Bali.Converter.Common.Extensions;
    using Bali.Converter.Common.Media;
    using Bali.Converter.YoutubeDl.Models;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    public class SingleMediaEditorViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IDownloadRegistry downloadRegistry;
        private readonly IMapper mapper;

        private VideoViewModel video;

        public SingleMediaEditorViewModel(IRegionManager regionManager, IDownloadRegistry downloadRegistry, IMapper mapper)
        {
            this.regionManager = regionManager;
            this.downloadRegistry = downloadRegistry;
            this.mapper = mapper;

            this.DownloadCommand = new DelegateCommand(this.Download);
        }

        public DelegateCommand DownloadCommand { get; }

        public VideoViewModel Video
        {
            get => this.video;
            set => this.SetProperty(ref this.video, value);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.Video = navigationContext.Parameters.GetValue<VideoViewModel>("Video");
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
            var job = new DownloadJob
            {
                Tags = this.mapper.Map<MediaTags>(this.Video.Tags),
                Url = this.Video.Url,
                ThumbnailPath = this.Video.ThumbnailPath,
                TargetFormat = Enum.Parse<MediaFormat>(this.Video.Format, true)
            };

            this.downloadRegistry.Add(job);

            this.regionManager.Regions["ContentRegion"].NavigationService.Journal.Clear();
            this.regionManager.Regions["ContentRegion"].RequestNavigate(nameof(MediaDownloaderView));
        }
    }
}
