namespace Bali.Converter.App
{
    using System.IO;
    using System.Reflection;
    using System.Windows;

    using AutoMapper;

    using Bali.Converter.App.Modules.Conversion.Video.View;
    using Bali.Converter.App.Modules.Conversion.Video.ViewModel;
    using Bali.Converter.App.Modules.Conversion.View;
    using Bali.Converter.App.Modules.Conversion.ViewModel;
    using Bali.Converter.App.Modules.Downloads;
    using Bali.Converter.App.Modules.Downloads.ViewModels;
    using Bali.Converter.App.Modules.Downloads.Views;
    using Bali.Converter.App.Modules.MediaDownloader.ViewModels;
    using Bali.Converter.App.Modules.MediaDownloader.Views;
    using Bali.Converter.App.Modules.Settings.ViewModels;
    using Bali.Converter.App.Modules.Settings.Views;
    using Bali.Converter.App.Profiles;
    using Bali.Converter.App.Services;
    using Bali.Converter.App.ViewModels;
    using Bali.Converter.App.Workers;
    using Bali.Converter.Common.Conversion;
    using Bali.Converter.FFmpeg;
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

            IFFmpeg ffmpeg = new FFmpeg(@"Tools\ffmpeg.exe");

            containerRegistry.RegisterInstance(DialogCoordinator.Instance);
            containerRegistry.RegisterInstance(youtubedl);
            containerRegistry.RegisterInstance(ffmpeg);
            containerRegistry.RegisterInstance(mapper);
            containerRegistry.RegisterSingleton<IConfigurationService, ConfigurationService>();
            containerRegistry.RegisterSingleton<IDownloadRegistry, DownloadRegistry>();

            containerRegistry.RegisterForNavigation<MediaDownloaderView, MediaDownloaderViewModel>();
            containerRegistry.RegisterForNavigation<SingleMediaEditorView, SingleMediaEditorViewModel>();
            containerRegistry.RegisterForNavigation<PlaylistSelectionView, PlaylistSelectionViewModel>();
            containerRegistry.RegisterForNavigation<PlaylistMediaEditorView, PlaylistMediaEditorViewModel>();
            containerRegistry.RegisterForNavigation<DownloadsView, DownloadsViewModel>();
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();

            containerRegistry.RegisterForNavigation<ConversionSelectionView, ConversionSelectionViewModel>();
            containerRegistry.RegisterForNavigation<VideoConversionEditorView, VideoConversionEditorViewModel>();

            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();

            containerRegistry.Register<DownloadBackgroundWorker>();

            containerRegistry.RegisterConversions();

            App.IsMinimized = false;
        }

        public static bool IsMinimized { get; set; }

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
