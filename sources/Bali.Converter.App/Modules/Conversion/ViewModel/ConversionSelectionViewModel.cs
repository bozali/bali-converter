namespace Bali.Converter.App.Modules.Conversion.ViewModel
{
    using System.Collections.ObjectModel;

    using Bali.Converter.App.Modules.Conversion.Video.View;
    using Bali.Converter.Common.Conversion;
    using Bali.Converter.Common.Enums;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    public class ConversionSelectionViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IConversionProvider conversionProvider;

        private ObservableCollection<string> supportedFormats;
        private string sourcePath;

        public ConversionSelectionViewModel(IRegionManager regionManager, IConversionProvider conversionProvider)
        {
            this.regionManager = regionManager;
            this.conversionProvider = conversionProvider;

            this.ContinueCommand = new DelegateCommand<string>(this.Continue);
            this.SupportedFormats = new ObservableCollection<string>();
        }

        public DelegateCommand<string> ContinueCommand { get; }

        public ObservableCollection<string> SupportedFormats
        {
            get => this.supportedFormats;
            set => this.SetProperty(ref this.supportedFormats, value);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.SupportedFormats = new ObservableCollection<string>();
            this.sourcePath = string.Empty;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void HandleDrop(string path)
        {
            this.SupportedFormats = new ObservableCollection<string>(this.conversionProvider.GetSupportedFormatsFor(path));
            this.sourcePath = path;
        }

        private void Continue(string target)
        {
            var conversion = this.conversionProvider.ProvideFor(target);

            var parameters = new NavigationParameters();
            parameters.Add("Conversion", conversion);
            parameters.Add("SourcePath", this.sourcePath);

            if (conversion.Topology.HasFlag(ConversionTopology.Video))
            {
                this.regionManager.RequestNavigate("ContentRegion", nameof(VideoConversionEditorView), parameters);
            }
        }
    }
}
