namespace Bali.Converter.App.Modules.Conversion.ViewModels
{
    using Prism.Mvvm;

    public class VideoConversionOptionsViewModel : BindableBase
    {
        private int maximumLength;
        private int fps;
        private int from;
        private int to;

        public VideoConversionOptionsViewModel()
        {
        }

        public int Fps
        {
            get => this.fps;
            set => this.SetProperty(ref this.fps, value);
        }

        public int From
        {
            get => this.@from;
            set => this.SetProperty(ref this.@from, value);
        }

        public int To
        {
            get => this.to;
            set => this.SetProperty(ref this.to, value);
        }

        public int MaximumLength
        {
            get => this.maximumLength;
            set => this.SetProperty(ref this.maximumLength, value);
        }
    }
}
