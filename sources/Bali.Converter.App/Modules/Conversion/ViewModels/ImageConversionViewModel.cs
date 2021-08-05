namespace Bali.Converter.App.Modules.Conversion.ViewModels
{
    using System.Threading.Tasks;

    using Bali.Converter.App.Modules.Conversion.Views;

    using ImageMagick;

    using Ookii.Dialogs.Wpf;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    public class ImageConversionViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;

        private string sourcePath;

        public ImageConversionViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;

            this.ConvertCommand = new DelegateCommand(async () => await this.Convert());
        }

        public DelegateCommand ConvertCommand { get; }

        public ConversionMetadata Metadata { get; set; }

        public string SourcePath
        {
            get => this.sourcePath;
            set => this.SetProperty(ref this.sourcePath, value);
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.Metadata = navigationContext.Parameters.GetValue<ConversionMetadata>("Metadata");
            this.SourcePath = this.Metadata.SourcePath;
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

                using var image = new MagickImage(this.Metadata.SourcePath);
                await image.WriteAsync(destination);
            }

            this.regionManager.RequestNavigate("ContentRegion", nameof(ConversionView));
        }
    }
}
