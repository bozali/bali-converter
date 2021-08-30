namespace Bali.Converter.App.Modules.MediaDownloader.ViewModels
{
    using Prism.Mvvm;

    public class VideoFormatViewModel : BindableBase
    {
        private int fps;
        private int height;
        private int width;
        private float abr;
        private float vbr;

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

        public int Fps
        {
            get => this.fps;
            set => this.SetProperty(ref this.fps, value);
        }

        public float AverageAudioBitRate
        {
            get => this.abr;
            set => this.SetProperty(ref this.abr, value);
        }

        public float AverageVideoBitRate
        {
            get => this.vbr;
            set => this.SetProperty(ref this.vbr, value);
        }
    }
}
