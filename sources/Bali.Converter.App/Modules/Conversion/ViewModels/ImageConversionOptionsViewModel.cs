namespace Bali.Converter.App.Modules.Conversion.ViewModels
{
    using Bali.Converter.App.Modules.Conversion.ViewModels.Image;

    using Prism.Mvvm;

    public class ImageConversionOptionsViewModel : BindableBase
    {
        private ResizingViewModel resizing;

        public ResizingViewModel Resizing
        {
            get => this.resizing;
            set => this.SetProperty(ref this.resizing, value);
        }
    }
}
