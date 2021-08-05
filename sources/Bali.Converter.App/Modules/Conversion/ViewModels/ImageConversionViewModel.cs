namespace Bali.Converter.App.Modules.Conversion.ViewModels
{
    using System;
    using System.Printing;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Bali.Converter.App.Modules.Conversion.ViewModels.Image;
    using Bali.Converter.App.Modules.Conversion.Views;

    using ImageMagick;

    using Ookii.Dialogs.Wpf;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    public class ImageConversionViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;

        private ImageConversionOptionsViewModel options;
        private string sourcePath;
        private MagickImage image;

        public ImageConversionViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;

            this.ConvertCommand = new DelegateCommand(async () => await this.Convert());
        }

        public DelegateCommand ConvertCommand { get; }

        public ConversionMetadata Metadata { get; set; }

        public ImageConversionOptionsViewModel Options
        {
            get => this.options;
            set => this.SetProperty(ref this.options, value);
        }

        public string SourcePath
        {
            get => this.sourcePath;
            set => this.SetProperty(ref this.sourcePath, value);
        }
        
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.Metadata = navigationContext.Parameters.GetValue<ConversionMetadata>("Metadata");
            this.SourcePath = this.Metadata.SourcePath;

            this.image = new MagickImage(this.SourcePath);

            this.Options = new ImageConversionOptionsViewModel();
            this.Options.Resizing = new ResizingViewModel
            {
                Option = ResizeOption.Standard,
                Width = this.image.Width,
                Height = this.image.Height,
                ResizeImage = false,
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
                await this.WriteImage(destination);
            }

            this.regionManager.RequestNavigate("ContentRegion", nameof(ConversionView));

            this.image?.Dispose();
        }

        // TODO Create a class for this conversion stuff.
        private async Task WriteImage(string destination)
        {
            if (this.Options.Resizing.ResizeImage)
            {
                switch (this.Options.Resizing.Option)
                {
                    case ResizeOption.Standard:
                        this.image.Resize(this.Options.Resizing.Width, this.Options.Resizing.Height);
                        break;
                    case ResizeOption.Adaptive:
                        this.image.AdaptiveResize(this.Options.Resizing.Width, this.Options.Resizing.Height);
                        break;
                    case ResizeOption.Interpolative:
                        this.image.InterpolativeResize(this.Options.Resizing.Width, this.Options.Resizing.Height, PixelInterpolateMethod.Bilinear);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            await this.image.WriteAsync(destination);
        }
    }
}
