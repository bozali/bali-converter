namespace Bali.Converter.App
{
    using Bali.Converter.App.Events;
    using Bali.Converter.App.Modules.MediaDownloader.Views;
    using Bali.Converter.App.ViewModels;

    using ControlzEx.Theming;

    using MahApps.Metro.Controls;

    using Prism.Events;
    using Prism.Regions;

    using Unity;

    public partial class MainWindow : MetroWindow
    {
        private readonly IRegionManager regionManager;

        public MainWindow(IRegionManager regionManager, IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.InitializeComponent();

            eventAggregator.GetEvent<WindowStateChangedEvent>().Subscribe(this.OnWindowStateChanged);

            this.Closing += async (s, e) =>
                            {
                                e.Cancel = true;
                                await ((MainWindowViewModel) this.DataContext).OnWindowClosing(s, e);
                            };

            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
            ThemeManager.Current.SyncTheme();

            this.regionManager = regionManager;

            this.regionManager.RegisterViewWithRegion("ContentRegion", () => container.Resolve<MediaDownloaderView>());
        }
        
        private void NavigationItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs args)
        {
            var item = (HamburgerMenuItem)args.InvokedItem;
            this.regionManager.Regions["ContentRegion"].RequestNavigate(item.Tag.ToString());
        }

        private void OnWindowStateChanged(bool show)
        {
            if (show)
            {
                this.Show();
                this.Focus();
                return;
            }

            this.Hide();
        }
    }
}
