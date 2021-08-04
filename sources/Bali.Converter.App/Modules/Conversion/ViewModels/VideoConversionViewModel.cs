namespace Bali.Converter.App.Modules.Conversion.ViewModels
{
    using System;
    using System.Windows.Controls;
    using Prism.Mvvm;
    using Prism.Regions;

    public class VideoConversionViewModel : BindableBase, INavigationAware
    {
        private VideoConversionOptionsViewModel options;
        private string sourcePath;

        public VideoConversionViewModel()
        {
        }

        public MediaElement MediaElement { get; set; }

        public ConversionMetadata Metadata { get; set; }

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

            this.Options = new VideoConversionOptionsViewModel
            {
                To = 0,
                From = 0,
                Fps = 30 // TODO Read the value from the metadata or from the file.
            };

            this.MediaElement.MediaOpened += (s, e) =>
                                             {
                                                 this.Options.MaximumLength = Convert.ToInt32(Math.Floor(this.MediaElement.NaturalDuration.TimeSpan.TotalSeconds));
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
    }
}
