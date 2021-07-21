﻿namespace Bali.Converter.App
{
    using System.Windows;
    using System.Windows.Controls;

    using Bali.Converter.App.Modules.MediaDownloader.Views;
    using ControlzEx.Theming;
    using MahApps.Metro.Controls;

    using Prism.Regions;

    using Unity;

    public partial class MainWindow : MetroWindow
    {
        private readonly IRegionManager regionManager;

        public MainWindow(IRegionManager regionManager, IUnityContainer container)
        {
            this.InitializeComponent();

            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
            ThemeManager.Current.SyncTheme();

            this.regionManager = regionManager;
            this.regionManager.RegisterViewWithRegion("ContentRegion", () => container.Resolve<MediaDownloaderView>());
        }

        private void NavigationButtonClicked(object sender, RoutedEventArgs e)
        {
            var item = (Button)sender;

            this.regionManager.Regions["ContentRegion"].RequestNavigate(item.Tag.ToString());
        }
    }
}