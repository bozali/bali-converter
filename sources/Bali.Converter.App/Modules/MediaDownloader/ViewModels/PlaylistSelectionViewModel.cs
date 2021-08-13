namespace Bali.Converter.App.Modules.MediaDownloader.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using AutoMapper;

    using Bali.Converter.App.Modules.Downloads;
    using Bali.Converter.App.Modules.MediaDownloader.Views;
    using Bali.Converter.Common.Enums;
    using Bali.Converter.Common.Extensions;
    using Bali.Converter.Common.Media;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    public class PlaylistSelectionViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IDownloadRegistry downloadRegistry;
        private readonly IMapper mapper;

        private ObservableCollection<VideoViewModel> videos;
        private string searchText;

        public PlaylistSelectionViewModel(IRegionManager regionManager, IDownloadRegistry downloadRegistry, IMapper mapper)
        {
            this.regionManager = regionManager;
            this.downloadRegistry = downloadRegistry;
            this.mapper = mapper;

            this.SelectVideoCommand = new DelegateCommand<VideoViewModel>(video => video.IsSelected = !video.IsSelected);
            this.EditVideoCommand = new DelegateCommand<VideoViewModel>(this.Edit);
            this.DownloadCommand = new DelegateCommand(this.Download);
            this.ToggleCommand = new DelegateCommand(this.Toggle);
        }

        public DelegateCommand<VideoViewModel> SelectVideoCommand { get; }

        public DelegateCommand<VideoViewModel> EditVideoCommand { get; }

        public DelegateCommand ToggleCommand { get; }

        public DelegateCommand DownloadCommand { get; }

        public string SearchText
        {
            get => this.searchText;
            set
            {
                if (this.SetProperty(ref this.searchText, value))
                {
                    this.RaisePropertyChanged(nameof(this.FilteredVideos));
                }
            }
        }

        public ObservableCollection<VideoViewModel> FilteredVideos
        {
            get
            {
                if (string.IsNullOrEmpty(this.SearchText))
                {
                    return this.Videos;
                }

                return new ObservableCollection<VideoViewModel>(this.Videos.Where(v => v.Tags.Title.Contains(this.SearchText)));
            }
        }

        public ObservableCollection<VideoViewModel> Videos
        {
            get => this.videos;
            set => this.SetProperty(ref this.videos, value);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.Videos = new ObservableCollection<VideoViewModel>(navigationContext.Parameters.GetValue<List<VideoViewModel>>("Videos"));
            this.Videos.ForEach(v => v.IsSelected = true);
            this.RaisePropertyChanged(nameof(this.FilteredVideos));
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
            foreach (var video in this.Videos.Where(v => v.IsSelected))
            {
                this.downloadRegistry.Add(new DownloadJob
                {
                    TargetFormat = Enum.Parse<FileExtension>(video.Format, true),
                    Tags = this.mapper.Map<MediaTags>(video.Tags),
                    ThumbnailPath = video.ThumbnailPath,
                    Url = video.Url,
                });
            }

            this.regionManager.Regions["ContentRegion"].NavigationService.Journal.Clear();
            this.regionManager.RequestNavigate("ContentRegion", nameof(MediaDownloaderView));
        }

        private void Toggle()
        {
            if (this.Videos.All(v => v.IsSelected) || this.Videos.All(v => !v.IsSelected))
            {
                this.Videos.ForEach(v => v.IsSelected = !v.IsSelected);
            }
            else if (this.Videos.Any(v => v.IsSelected))
            {
                this.Videos.ForEach(v => v.IsSelected = true);
            }
        }

        private void Edit(VideoViewModel video)
        {
            var parameters = new NavigationParameters();
            parameters.Add("Video", video);

            this.regionManager.Regions["ContentRegion"].RequestNavigate(nameof(PlaylistMediaEditorView), parameters);
        }
    }
}
