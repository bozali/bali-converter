namespace Bali.Converter.App.Modules.Conversion.Video.ViewModel
{
    using Prism.Mvvm;

    public class VideoConversionOptionsViewModel : BindableBase
    {
        private int quality;

        public int Quality
        {
            get => this.quality;
            set => this.SetProperty(ref this.quality, value);
        }
    }
}
