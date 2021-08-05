namespace Bali.Converter.App.Modules.Conversion.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Threading;

    using Bali.Converter.App.Modules.Conversion.Views;
    using Bali.Converter.FFmpeg;
    using Bali.Converter.FFmpeg.Models;

    using Ookii.Dialogs.Wpf;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    public class VideoConversionViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IFFmpeg ffmpeg;

        private VideoConversionOptionsViewModel options;
        private DispatcherTimer timer;
        private string sourcePath;
        private bool isMediaPlaying;
        private int mediaPosition;

        public VideoConversionViewModel(IRegionManager regionManager, IFFmpeg ffmpeg)
        {
            this.regionManager = regionManager;
            this.ffmpeg = ffmpeg;

            this.ConvertCommand = new DelegateCommand(async () => await this.Convert());
            this.PlayPauseCommand = new DelegateCommand(this.PlayPauseMedia);
            this.StopCommand = new DelegateCommand(this.StopMedia);

            this.IsMediaPlaying = true;

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(1.0);
            this.timer.Tick += (s, e) => this.MediaPosition = this.MediaElement.Position.Seconds;
            this.timer.Start();
        }

        public DelegateCommand ConvertCommand { get; }

        public DelegateCommand PlayPauseCommand { get; }

        public DelegateCommand StopCommand { get; }

        public MediaElement MediaElement { get; set; }

        public ConversionMetadata Metadata { get; set; }

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

        public string SourcePath
        {
            get => this.sourcePath;
            set => this.SetProperty(ref this.sourcePath, value);
        }

        public VideoConversionOptionsViewModel Options
        {
            get => this.options;
            set => this.SetProperty(ref this.options, value);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.Metadata = navigationContext.Parameters.GetValue<ConversionMetadata>("Metadata");
            this.SourcePath = this.Metadata.SourcePath;

            // TODO Read the information from the SourcePath

            this.Options = new VideoConversionOptionsViewModel
            {
                To = 0,
                From = 0,
                Fps = 60 // TODO Read the value from the metadata or from the file.
            };

            this.MediaElement.MediaOpened += (s, e) =>
                                             {
                                                 this.Options.MaximumLength = System.Convert.ToInt32(Math.Floor(this.MediaElement.NaturalDuration.TimeSpan.TotalSeconds));
                                                 this.Options.To = this.Options.MaximumLength;
                                             };
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
            string extension = this.Metadata.Target.ToLower();
            string filter = $"(*.{extension})|*.*";

            var dialog = new VistaSaveFileDialog();
            dialog.Filter = filter;
            dialog.DefaultExt = $".{extension}";

            if (dialog.ShowDialog() ?? false)
            {
                string destination = dialog.FileName;
                await this.ffmpeg.Convert(this.Metadata.SourcePath, destination, new VideoConversionOptions
                {
                    To = this.Options.To,
                    From = this.Options.From,
                    Fps = this.Options.Fps
                });
            }

            this.regionManager.RequestNavigate("ContentRegion", nameof(ConversionView));
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
