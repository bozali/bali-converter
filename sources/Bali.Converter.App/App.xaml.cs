namespace Bali.Converter.App
{
    using System.Windows;

    using Bali.Converter.App.Modules.MediaDownloader.ViewModels;
    using Bali.Converter.App.Modules.MediaDownloader.Views;

    using Prism.Ioc;
    using Prism.Unity;

    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MediaDownloaderView, MediaDownloaderViewModel>();
        }

        protected override Window CreateShell()
        {
            return this.Container.Resolve<MainWindow>();
        }
    }
}
