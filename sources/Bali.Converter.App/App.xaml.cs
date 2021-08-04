namespace Bali.Converter.App
{
    using System.IO;
    using System.Reflection;
    using System.Windows;

    using AutoMapper;

    using Bali.Converter.App.Modules.Conversion.ViewModels;
    using Bali.Converter.App.Modules.Conversion.Views;
    using Bali.Converter.App.Modules.Downloads;
    using Bali.Converter.App.Modules.Downloads.ViewModels;
    using Bali.Converter.App.Modules.Downloads.Views;
    using Bali.Converter.App.Modules.MediaDownloader.ViewModels;
    using Bali.Converter.App.Modules.MediaDownloader.Views;
    using Bali.Converter.App.Modules.Settings.ViewModels;
    using Bali.Converter.App.Modules.Settings.Views;
    using Bali.Converter.App.Profiles;
    using Bali.Converter.App.Services;
    using Bali.Converter.App.Workers;
    using Bali.Converter.YoutubeDl;

    using log4net;
    using log4net.Config;

    using MahApps.Metro.Controls.Dialogs;

    using Prism.Ioc;
    using Prism.Unity;

    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var repository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));

            IYoutubeDl youtubedl = new YoutubeDl(@"Tools\youtube-dl.exe", @"Tools\ffmpeg.exe", IConfigurationService.TempPath);
            var mapper = new MapperConfiguration(configuration => configuration.AddProfile<AutoMapperProfile>()).CreateMapper();

            containerRegistry.RegisterInstance(DialogCoordinator.Instance);
            containerRegistry.RegisterInstance(youtubedl);
            containerRegistry.RegisterInstance(mapper);
            containerRegistry.RegisterSingleton<IConfigurationService, ConfigurationService>();
            containerRegistry.RegisterSingleton<IDownloadRegistry, DownloadRegistry>();

            containerRegistry.RegisterForNavigation<MediaDownloaderView, MediaDownloaderViewModel>();
            containerRegistry.RegisterForNavigation<SingleMediaEditorView, SingleMediaEditorViewModel>();
            containerRegistry.RegisterForNavigation<PlaylistSelectionView, PlaylistSelectionViewModel>();
            containerRegistry.RegisterForNavigation<PlaylistMediaEditorView, PlaylistMediaEditorViewModel>();
            containerRegistry.RegisterForNavigation<DownloadsView, DownloadsViewModel>();
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();

            containerRegistry.RegisterForNavigation<ConversionView, ConversionViewModel>();
            containerRegistry.RegisterForNavigation<VideoConversionView, VideoConversionViewModel>();

            containerRegistry.Register<DownloadBackgroundWorker>();
        }

        protected override Window CreateShell()
        {
            return this.Container.Resolve<MainWindow>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            this.Container.Resolve<DownloadBackgroundWorker>().Process().ConfigureAwait(false);
        }
    }
}
