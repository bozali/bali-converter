namespace Bali.Converter.App.Modules.Conversion.Video.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Controls;
    using System.Windows.Threading;

    using Bali.Converter.App.Modules.Conversion.Filters.ViewModels;
    using Bali.Converter.App.Modules.Conversion.Filters.Views;
    using Bali.Converter.Common.Conversion;
    using Bali.Converter.Common.Enums;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    using Unity;

    public class VideoConversionEditorViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IUnityContainer container;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly DispatcherTimer timer;

        private VideoConversionOptionsViewModel videoConversionOptions;
        private VideoMetadataViewModel metadata;
        private FilterBaseViewModel selectedFilter;

        private ObservableCollection<FilterBaseViewModel> filters;
        private ObservableCollection<string> supportedFilters;

        private IConversion conversion;
        private string sourcePath;
        private bool isMediaPlaying;
        private int mediaPosition;

        public VideoConversionEditorViewModel(IRegionManager regionManager, IUnityContainer container)
        {
            this.regionManager = regionManager;
            this.container = container;
            this.IsMediaPlaying = true;

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(1.0);
            this.timer.Tick += (s, e) => this.MediaPosition = this.MediaElement.Position.Seconds;
            this.timer.Start();

            this.PlayPauseCommand = new DelegateCommand(this.PlayPauseMedia);
            this.StopCommand = new DelegateCommand(this.StopMedia);

            this.Filters = new ObservableCollection<FilterBaseViewModel>();
            this.Filters.Add(new VolumeFilterViewModel());
            this.Filters.Add(new RotationFilterViewModel());

            this.RaisePropertyChanged(nameof(this.Filters));
        }

        public DelegateCommand PlayPauseCommand { get; }

        public DelegateCommand StopCommand { get; }

        public MediaElement MediaElement { get; set; }

        public ObservableCollection<FilterBaseViewModel> Filters
        {
            get => this.filters;
            set => this.SetProperty(ref this.filters, value);
        }

        public FilterBaseViewModel SelectedFilter
        {
            get => this.selectedFilter;
            set
            {
                this.SetProperty(ref this.selectedFilter, value);

                var region = this.regionManager.Regions["FilterRegion"];
                region.RemoveAll();
                region.Context = value;

                UserControl view = null;

                if (value is VolumeFilterViewModel)
                {
                    view = this.container.Resolve<VolumeFilterView>();
                }
                else if (value is RotationFilterViewModel)
                {
                    view = this.container.Resolve<RotationFilterView>();
                }

                region.Add(view);
                region.Activate(view);
            }
        }

        public ObservableCollection<string> SupportedFilters
        {
            get => this.supportedFilters;
            set => this.SetProperty(ref this.supportedFilters, value);
        }

        public VideoMetadataViewModel Metadata
        {
            get => this.metadata;
            set => this.SetProperty(ref this.metadata, value);
        }

        public bool ContainsAudioOptions
        {
            get => this.conversion.Topology.HasFlag(ConversionTopology.Audio);
        }

        public bool ContainsVideoOptions
        {
            get => this.conversion.Topology.HasFlag(ConversionTopology.Video);
        }

        public string SourcePath
        {
            get => this.sourcePath;
            set => this.SetProperty(ref this.sourcePath, value);
        }

        public VideoConversionOptionsViewModel VideoConversionOptions
        {
            get => this.videoConversionOptions;
            set => this.SetProperty(ref this.videoConversionOptions, value);
        }

        // TODO We need a custom control for a media player.
        public bool IsMediaPlaying
        {
            get => this.isMediaPlaying;
            set => this.SetProperty(ref this.isMediaPlaying, value);
        }

        public int MediaPosition
        {
            get => this.mediaPosition;
            set => this.SetProperty(ref this.mediaPosition, value);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.conversion = navigationContext.Parameters.GetValue<IConversion>("Conversion");
            this.SourcePath = navigationContext.Parameters.GetValue<string>("SourcePath");

            this.Metadata = new VideoMetadataViewModel();

            this.MediaElement.MediaOpened += (s, e) =>
                                             {
                                                 this.metadata.MaximumLength = System.Convert.ToInt32(this.MediaElement.NaturalDuration.TimeSpan.TotalSeconds);
                                             };

            this.SupportedFilters = new ObservableCollection<string>(new List<string>
            {
                "Volume",
                "Rotation",
            });
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private void PlayPauseMedia()
        {
            if (this.IsMediaPlaying)
            {
                this.MediaElement.Pause();
            }
            else
            {
                this.MediaElement.Play();
            }

            this.IsMediaPlaying = !this.IsMediaPlaying;
        }

        private void StopMedia()
        {
            this.MediaElement.Stop();
            this.IsMediaPlaying = false;
        }
    }
}
