namespace Bali.Converter.App.Modules.Conversion.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Bali.Converter.App.Modules.Conversion.Views;
    using MahApps.Metro.Controls.Dialogs;
    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    public class ConversionViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IDialogCoordinator dialog;

        private ObservableCollection<string> supported;

        public ConversionViewModel(IRegionManager regionManager, IDialogCoordinator dialog)
        {
            this.regionManager = regionManager;
            this.dialog = dialog;

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

        public async Task HandleDrop(string path)
        {
            this.Metadata = ConversionMetadataFactory.CreateMetadata(path);

            if (this.Metadata == null)
            {
                await this.dialog.ShowMessageAsync(this, "Not supported", $"The file format of {path} is not supported.");
                return;
            }

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
