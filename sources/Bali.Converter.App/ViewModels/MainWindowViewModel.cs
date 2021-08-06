namespace Bali.Converter.App.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using Bali.Converter.App.Events;
    using Bali.Converter.App.Services;

    using MahApps.Metro.Controls.Dialogs;

    using Prism.Events;
    using Prism.Mvvm;

    public class MainWindowViewModel : BindableBase
    {
        private readonly IConfigurationService configurationService;
        private readonly IEventAggregator eventAggregator;
        private readonly IDialogCoordinator dialog;

        private NotifyIcon icon;

        public MainWindowViewModel(IConfigurationService configurationService, IEventAggregator eventAggregator, IDialogCoordinator dialog)
        {
            this.configurationService = configurationService;
            this.eventAggregator = eventAggregator;
            this.dialog = dialog;
        }

        public async Task OnWindowClosing(object sender, CancelEventArgs args)
        {
            var configuration = this.configurationService.Configuration;

            if (configuration.FirstTime)
            {
                var result = await this.dialog.ShowMessageAsync(this, "Application closing", "Do you want to minimize the application?",
                                                                MessageDialogStyle.AffirmativeAndNegative,
                                                                new MetroDialogSettings
                                                                {
                                                                    AffirmativeButtonText = "Yes",
                                                                    NegativeButtonText = "No"
                                                                });

                configuration.Minimize = result == MessageDialogResult.Affirmative;
                configuration.FirstTime = false;

                this.configurationService.Save(configuration);
            }

            if (configuration.Minimize)
            {
                this.InitializeNotifyIcon();
                this.eventAggregator.GetEvent<WindowStateChangedEvent>().Publish(false);
            }
            else
            {
                this.configurationService.Save(configuration);
                App.Current.Shutdown();
            }
        }

        private void InitializeNotifyIcon()
        {
            if (this.icon == null)
            {
                this.icon = new NotifyIcon();
                this.icon.Icon = SystemIcons.Application;
                this.icon.BalloonTipText = "Bali Converter";
                this.icon.Visible = true;
                
                this.icon.MouseDoubleClick += this.OnNotifyIconMouseDoubleClicked;

                var contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add("Exit", null, this.OnContextMenuExitClicked);

                this.icon.ContextMenuStrip = contextMenu;
            }
        }

        private void OnContextMenuExitClicked(object sender, EventArgs e)
        {
            // TODO Check if any download is being processed and get user confirmation.
            // ReSharper disable once AccessToStaticMemberViaDerivedType
            App.Current.Shutdown();
        }

        private void OnNotifyIconMouseDoubleClicked(object? sender, MouseEventArgs e)
        {
            this.eventAggregator.GetEvent<WindowStateChangedEvent>().Publish(true);
        }
    }
}
