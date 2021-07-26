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

        private MediaTagsViewModel mediaTags;
        private string format;
        private byte[] thumbnailData;
        private string thumbnailPath;
        private Video video;

        public SingleMediaEditorViewModel(IRegionManager regionManager, IDownloadRegistry downloadRegistry, IMapper mapper)
        {
            this.regionManager = regionManager;
            this.downloadRegistry = downloadRegistry;
            this.mapper = mapper;

            this.DownloadCommand = new DelegateCommand(this.Download);
        }

        public DelegateCommand DownloadCommand { get; }

        public MediaTagsViewModel MediaTags
        {
            get => this.mediaTags;
            set => this.SetProperty(ref this.mediaTags, value);
        }

        public string Format
        {
            get => this.format;
            set => this.SetProperty(ref this.format, value);
        }

        public byte[] ThumbnailData
        {
            get => this.thumbnailData;
            set => this.SetProperty(ref this.thumbnailData, value);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.video = navigationContext.Parameters.GetValue<Video>("Video");

            this.Format = navigationContext.Parameters.GetValue<string>("Format");

            this.thumbnailPath = navigationContext.Parameters.GetValue<string>("ThumbnailPath");
            this.ThumbnailData = File.ReadAllBytes(this.thumbnailPath);

            this.MediaTags = new MediaTagsViewModel
            {
                Title = this.video.Title.RemoveIllegalChars(),
                Artist = this.video.Channel,
                Year = this.video.UploadDate.Year
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
                Tags = this.mapper.Map<MediaTags>(this.MediaTags),
                Url = this.video.Url,
                ThumbnailPath = this.thumbnailPath,
                TargetFormat = Enum.Parse<MediaFormat>(this.Format, true)
            };

            this.downloadRegistry.Add(job);

            this.regionManager.Regions["ContentRegion"].NavigationService.Journal.Clear();
            this.regionManager.Regions["ContentRegion"].RequestNavigate(nameof(MediaDownloaderView));
        }
    }
}
