namespace Bali.Converter.App.Modules.MediaDownloader.ViewModels
{
    using AutoMapper;
    using Bali.Converter.App.Modules.MediaDownloader.Views;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    public class PlaylistMediaEditorViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IMapper mapper;

        private VideoViewModel original;
        private VideoViewModel video;

        public PlaylistMediaEditorViewModel(IRegionManager regionManager, IMapper mapper)
        {
            this.regionManager = regionManager;
            this.mapper = mapper;

            this.SaveCommand = new DelegateCommand(this.Save);
            this.CancelCommand = new DelegateCommand(this.Cancel);
        }

        public DelegateCommand SaveCommand { get; }

        public DelegateCommand CancelCommand { get; }

        public VideoViewModel Video
        {
            get => this.video;
            set => this.SetProperty(ref this.video, value);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.original = navigationContext.Parameters.GetValue<VideoViewModel>("Video");

            this.Video = this.mapper.Map<VideoViewModel>(this.original);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private void Save()
        {
            this.original = this.Video;
            this.regionManager.Regions["ContentRegion"].NavigationService.Journal.GoBack();
        }

        private void Cancel()
        {
            this.regionManager.Regions["ContentRegion"].NavigationService.Journal.GoBack();
        }
    }
}
