namespace Bali.Converter.App.Modules.Conversion.Video.ViewModel
{
    using Bali.Converter.Common.Conversion;
    using Bali.Converter.Common.Enums;
    using Prism.Mvvm;
    using Prism.Regions;

    public class VideoConversionEditorViewModel : BindableBase, INavigationAware
    {
        private IConversion conversion;
        private string sourcePath;

        public VideoConversionEditorViewModel()
        {
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

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.conversion = navigationContext.Parameters.GetValue<IConversion>("Conversion");
            this.SourcePath = navigationContext.Parameters.GetValue<string>("SourcePath");
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
