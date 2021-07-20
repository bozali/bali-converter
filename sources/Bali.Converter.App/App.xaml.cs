namespace Bali.Converter.App
{
    using System.Windows;

    using Bali.Converter.App.Modules.MediaDownloader.ViewModels;
    using Bali.Converter.App.Modules.MediaDownloader.Views;
    using Bali.Converter.App.Modules.Settings.ViewModels;
    using Bali.Converter.App.Modules.Settings.Views;
    using Bali.Converter.App.Services;
    using Bali.Converter.YoutubeDl;

    using Prism.Ioc;
    using Prism.Unity;

    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            IYoutubeDl youtubedl = new YoutubeDl(@"Tools\youtube-dl.exe", @"Tools\ffmpeg.exe", IConfigurationService.TempPath);

            containerRegistry.RegisterInstance(youtubedl);
            containerRegistry.Register<IConfigurationService, ConfigurationService>();

            containerRegistry.RegisterForNavigation<MediaDownloaderView, MediaDownloaderViewModel>();
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();
        }

        protected override Window CreateShell()
        {
            return this.Container.Resolve<MainWindow>();
        }
    }
}
