namespace Bali.Converter.App.Modules.Conversion.Video.ViewModel
{
    using Prism.Mvvm;

    public class VideoMetadataViewModel : BindableBase
    {
        private int maximumLength;

        public int MaximumLength
        {
            get => this.maximumLength;
            set => this.SetProperty(ref this.maximumLength, value);
        }
    }
}
