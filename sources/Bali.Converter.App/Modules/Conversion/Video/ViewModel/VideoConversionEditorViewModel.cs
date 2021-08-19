namespace Bali.Converter.App.Modules.Conversion.Video.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using AutoMapper;
    using Bali.Converter.App.Modules.Conversion.Filters;
    using Bali.Converter.App.Modules.Conversion.Filters.ViewModels;
    using Bali.Converter.App.Modules.Conversion.Filters.Views;
    using Bali.Converter.App.Modules.Conversion.View;
    using Bali.Converter.Common.Conversion;
    using Bali.Converter.Common.Conversion.Audio;
    using Bali.Converter.Common.Conversion.Video;
    using Bali.Converter.Common.Enums;
    using Bali.Converter.FFmpeg.Filters.Audio;
    using Bali.Converter.FFmpeg.Filters.Video;
    using MahApps.Metro.Controls.Dialogs;
    using Ookii.Dialogs.Wpf;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    using Unity;

    public class VideoConversionEditorViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IUnityContainer container;
        private readonly IMapper mapper;
        private readonly IDialogCoordinator dialog;

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

        public VideoConversionEditorViewModel(IRegionManager regionManager,
                                              IUnityContainer container,
                                              IMapper mapper,
                                              IDialogCoordinator dialog)
        {
            this.regionManager = regionManager;
            this.container = container;
            this.mapper = mapper;
            this.dialog = dialog;
            this.IsMediaPlaying = true;

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(1.0);
            this.timer.Tick += (s, e) =>
                               {
                                   this.MediaPosition = this.MediaElement.Position.Seconds;
                                   this.RaisePropertyChanged(nameof(this.Position));
                               };
            this.timer.Start();

            this.ConvertCommand = new DelegateCommand(async () => await this.Convert());
            this.PlayPauseCommand = new DelegateCommand(this.PlayPauseMedia);
            this.StopCommand = new DelegateCommand(this.StopMedia);
            this.AddFilterCommand = new DelegateCommand<string>(this.AddFilter);
            this.RemoveFilterCommand = new DelegateCommand<FilterBaseViewModel>(this.RemoveFilter);

            this.Filters = new ObservableCollection<FilterBaseViewModel>();
            this.VideoConversionOptions = new VideoConversionOptionsViewModel();
        }

        public DelegateCommand ConvertCommand { get; }

        public DelegateCommand PlayPauseCommand { get; }

        public DelegateCommand StopCommand { get; }

        public DelegateCommand<string> AddFilterCommand { get; }

        public DelegateCommand<FilterBaseViewModel> RemoveFilterCommand { get; }

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

                UserControl view = null;

                if (value is VolumeFilterViewModel)
                {
                    view = this.container.Resolve<VolumeFilterView>();
                }
                else if (value is RotationFilterViewModel)
                {
                    view = this.container.Resolve<RotationFilterView>();
                }
                else if (value is FpsFilterViewModel)
                {
                    view = this.container.Resolve<FpsFilterView>();
                }
                else
                {
                    return;
                }

                view.DataContext = value;

                region.Context = value;
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

        public TimeSpan Position
        {
            get => this.MediaElement?.Position ?? TimeSpan.Zero;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.Filters.Clear();

            this.conversion = navigationContext.Parameters.GetValue<IConversion>("Conversion");
            this.SourcePath = navigationContext.Parameters.GetValue<string>("SourcePath");

            this.Metadata = new VideoMetadataViewModel();

            this.MediaElement.MediaOpened += (s, e) =>
                                             {
                                                 this.metadata.MaximumLength = System.Convert.ToInt32(this.MediaElement.NaturalDuration.TimeSpan.TotalSeconds);

                                                 Debug.WriteLine(this.MediaElement.NaturalDuration.TimeSpan.TotalSeconds);

                                                 this.VideoConversionOptions.MinVideoLength = 0.0;
                                                 this.VideoConversionOptions.MaxVideoLength = this.MediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                                             };

            this.SupportedFilters = new ObservableCollection<string>();


            if (this.conversion.Topology.HasFlag(ConversionTopology.Audio))
            {
                this.SupportedFilters.AddRange(new[] { FilterNameConstants.Audio.Volume });
            }

            if (this.conversion.Topology.HasFlag(ConversionTopology.Video))
            {
                this.SupportedFilters.AddRange(new[] { FilterNameConstants.Video.Rotation, FilterNameConstants.Video.Fps });
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private async Task Convert()
        {
            if (this.conversion.Topology.HasFlag(ConversionTopology.Video))
            {
                ((IVideoConversion)this.conversion).VideoConversionOptions = new VideoConversionOptions
                {
                    VideoFilters = this.GetVideoFilters().ToArray()
                };
            }

            if (this.conversion.Topology.HasFlag(ConversionTopology.Audio))
            {
                ((IAudioConversion)this.conversion).AudioConversionOptions = new AudioConversionOptions
                {
                    AudioFilters = this.GetAudioFilters().ToArray()
                };
            }


            var fileDialog = new VistaSaveFileDialog();
            fileDialog.Filter = $"(*.{this.conversion.Extension.ToLowerInvariant()})|*.*";
            fileDialog.DefaultExt = $".{this.conversion.Extension}";

            if (fileDialog.ShowDialog() ?? false)
            {
                var progress = await this.dialog.ShowProgressAsync(this, "Converting...", "Please wait...");

                try
                {
                    string destination = fileDialog.FileName;
                    await this.conversion.Convert(this.SourcePath, destination);

                    this.regionManager.RequestNavigate("ContentRegion", nameof(ConversionSelectionView));
                }
                catch (Exception e)
                {
                    progress.SetMessage(e.Message);
                }
                finally
                {
                    await progress.CloseAsync();
                }
            }
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

        private void AddFilter(string filter)
        {
            // TODO Register these types 
            switch (filter)
            {
                case "Rotation":
                    this.Filters.Add(this.container.Resolve(typeof(RotationFilterViewModel)) as FilterBaseViewModel);
                    break;

                case "Volume":
                    this.Filters.Add(this.container.Resolve(typeof(VolumeFilterViewModel)) as FilterBaseViewModel);
                    break;
            }
        }

        private void RemoveFilter(FilterBaseViewModel filter)
        {
            this.Filters.Remove(filter);
        }

        private IEnumerable<IVideoFilter> GetVideoFilters()
        {
            foreach (var filter in this.Filters)
            {
                if (filter.DisplayName == FilterNameConstants.Video.Rotation)
                {
                    yield return this.mapper.Map<RotationFilterViewModel, RotationFilter>((RotationFilterViewModel)filter);
                }
                else if (filter.DisplayName == FilterNameConstants.Video.Fps)
                {
                    yield return this.mapper.Map<FpsFilterViewModel, FpsFilter>((FpsFilterViewModel)filter);
                }
            }
        }

        private IEnumerable<IAudioFilter> GetAudioFilters()
        {
            foreach (var filter in this.Filters)
            {
                if (filter.DisplayName == FilterNameConstants.Audio.Volume)
                {
                    yield return this.mapper.Map<VolumeFilterViewModel, VolumeFilter>((VolumeFilterViewModel)filter);
                }
            }
        }
    }
}
