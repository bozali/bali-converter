namespace Bali.Converter.App
{
    using System.IO;
    using System.Windows;

    using Bali.Converter.App.Modules.MediaDownloader.ViewModels;
    using Bali.Converter.App.Modules.MediaDownloader.Views;
    using Bali.Converter.YoutubeDl;

    using Prism.Ioc;
    using Prism.Unity;

    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "bali-converter");

            IYoutubeDl youtubedl = new YoutubeDl(@"Tools\youtube-dl.exe", @"Tools\ffmpeg.exe", tempPath);

            containerRegistry.RegisterInstance(youtubedl);

            containerRegistry.RegisterForNavigation<MediaDownloaderView, MediaDownloaderViewModel>();
        }

        protected override Window CreateShell()
        {
            return this.Container.Resolve<MainWindow>();
        }
    }
}
