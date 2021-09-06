namespace Bali.Converter.App.Modules.MediaDownloader.ViewModels
{
    using System;

    using AutoMapper;

    using Bali.Converter.App.Modules.Downloads;
    using Bali.Converter.App.Modules.MediaDownloader.Views;
    using Bali.Converter.App.Services;
    using Bali.Converter.Common.Enums;
    using Bali.Converter.Common.Media;
    using Bali.Converter.YoutubeDl.Quality;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    public class SingleMediaEditorViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IDownloadRegistryService downloadRegistry;
        private readonly IMapper mapper;

        private QualityOptionViewModel quality;
        private VideoViewModel video;

        public SingleMediaEditorViewModel(IRegionManager regionManager, IDownloadRegistryService downloadRegistry, IMapper mapper)
        {
            this.regionManager = regionManager;
            this.downloadRegistry = downloadRegistry;
            this.mapper = mapper;

            this.DownloadCommand = new DelegateCommand(this.Download);
        }

        public DelegateCommand DownloadCommand { get; }

        public QualityOptionViewModel Quality
        {
            get => this.quality;
            set => this.SetProperty(ref this.quality, value);
        }

        public VideoViewModel Video
        {
            get => this.video;
            set => this.SetProperty(ref this.video, value);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.Video = navigationContext.Parameters.GetValue<VideoViewModel>("Video");

            this.Quality = new QualityOptionViewModel
            {
                VideoQualityOption = navigationContext.Parameters.GetValue<AutomaticQualityOption>("VideoQuality"),
                AudioQualityOption = navigationContext.Parameters.GetValue<AutomaticQualityOption>("AudioQuality")
            };
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
                TargetFormat = Enum.Parse<FileExtension>(this.Video.Format, true)
            };

            this.downloadRegistry.Add(job);

            this.regionManager.Regions["ContentRegion"].NavigationService.Journal.Clear();
            this.regionManager.Regions["ContentRegion"].RequestNavigate(nameof(MediaDownloaderView));
        }
    }
}
