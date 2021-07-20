namespace Bali.Converter.App.Modules.MediaDownloader.ViewModels
{
    using System.Windows.Input;
    using Prism.Commands;
    using Prism.Mvvm;

    public class MediaDownloaderViewModel : BindableBase
    {
        private string url;

        public MediaDownloaderViewModel()
        {
            this.DownloadCommand = new DelegateCommand(this.Download, () => !string.IsNullOrEmpty(this.Url));
        }

        public string Url
        {
            get => this.url;
            set => this.SetProperty(ref this.url, value);
        }

        public ICommand DownloadCommand { get; }

        private void Download()
        {
        }
    }
}
