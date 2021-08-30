namespace Bali.Converter.App.Modules.MediaDownloader.ViewModels
{
    using Bali.Converter.YoutubeDl.Quality;

    using Prism.Mvvm;

    public class QualityOptionViewModel : BindableBase
    {
        private AutomaticQualityOption audioQualityOption;
        private AutomaticQualityOption videoQualityOption;

        public AutomaticQualityOption AudioQualityOption
        {
            get => this.audioQualityOption;
            set => this.SetProperty(ref this.audioQualityOption, value);
        }

        public AutomaticQualityOption VideoQualityOption
        {
            get => this.videoQualityOption;
            set => this.SetProperty(ref this.videoQualityOption, value);
        }
    }
}
