namespace Bali.Converter.App.Modules.Conversion.ViewModels.Image
{
    using Prism.Mvvm;

    public class ResizingViewModel : BindableBase
    {
        private ResizeOption option;
        private bool resizeImage;
        private int width;
        private int height;

        public ResizeOption Option
        {
            get => this.option;
            set => this.SetProperty(ref this.option, value);
        }

        public bool ResizeImage
        {
            get => this.resizeImage;
            set => this.SetProperty(ref this.resizeImage, value);
        }

        public int Width
        {
            get => this.width;
            set => this.SetProperty(ref this.width, value);
        }

        public int Height
        {
            get => this.height;
            set => this.SetProperty(ref this.height, value);
        }
    }
}
