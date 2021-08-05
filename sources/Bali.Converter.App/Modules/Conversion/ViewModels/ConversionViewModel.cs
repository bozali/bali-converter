namespace Bali.Converter.App.Modules.Conversion.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;

    using Bali.Converter.App.Modules.Conversion.Views;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    public class ConversionViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;

        private ObservableCollection<string> supported;

        public ConversionViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;

            this.Supported = new ObservableCollection<string>();
            this.ContinueCommand = new DelegateCommand<string>(this.Continue);
        }

        public DelegateCommand<string> ContinueCommand { get; }

        public ObservableCollection<string> Supported
        {
            get => this.supported;
            set => this.SetProperty(ref this.supported, value);
        }

        public ConversionMetadata Metadata { get; set; }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.Supported = new ObservableCollection<string>();
            this.Metadata = null;
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
            this.Metadata = ConversionMetadataFactory.CreateMetadata(path);

            this.Supported = new ObservableCollection<string>(this.Metadata.SupportedTargetFormats.Select(c => c.ToString("G")));
        }

        private void Continue(string target)
        {
            this.Metadata.Target = target;

            var parameters = new NavigationParameters();
            parameters.Add("Metadata", this.Metadata);

            this.regionManager.RequestNavigate("ContentRegion", nameof(VideoConversionView), parameters);
        }
    }
}
