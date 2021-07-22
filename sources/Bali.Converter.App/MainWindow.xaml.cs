namespace Bali.Converter.App
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
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
        
        private void NavigationItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs args)
        {
            var item = (HamburgerMenuItem)args.InvokedItem;
            this.regionManager.Regions["ContentRegion"].RequestNavigate(item.Tag.ToString());
        }

        public IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    if (child != null && child is T)
                        yield return (T)child;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }
    }
}
