namespace Bali.Converter.App.Modules.Settings.ViewModels
{
    using Bali.Converter.App.Services;

    using Ookii.Dialogs.Wpf;

    using Prism.Commands;
    using Prism.Mvvm;

    public class SettingsViewModel : BindableBase
    {
        private readonly IConfigurationService configurationService;

        private string downloadDir;
        private int bandwidthMinimized;
        private int bandwidth;
        private bool minimize;

        public SettingsViewModel(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;

            var configuration = this.configurationService.Configuration;

            this.DownloadDir = configuration.DownloadDir;
            this.Minimize = configuration.Minimize;
            this.Bandwidth = configuration.Bandwidth;
            this.BandwidthMinimized = configuration.BandwidthMinimized;

            this.SaveCommand = new DelegateCommand(this.Save, () => this.HasChanges);
            this.SelectDownloadDirCommand = new DelegateCommand(this.SelectDownloadDir);

            this.RaisePropertyChanged();
        }

        public DelegateCommand SaveCommand { get; }

        public DelegateCommand SelectDownloadDirCommand { get; }

        public string DownloadDir
        {
            get => this.downloadDir;
            set
            {
                if (this.SetProperty(ref this.downloadDir, value))
                {
                    this.RaisePropertyChanged(nameof(this.HasChanges));
                    this.SaveCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        public bool Minimize
        {
            get => this.minimize;
            set
            {
                if (this.SetProperty(ref this.minimize, value))
                {
                    this.RaisePropertyChanged(nameof(this.HasChanges));
                    this.SaveCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        public int Bandwidth
        {
            get => this.bandwidth;
            set
            {
                if (this.SetProperty(ref this.bandwidth, value))
                {
                    this.RaisePropertyChanged(nameof(this.HasChanges));
                    this.SaveCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        public int BandwidthMinimized
        {
            get => this.bandwidthMinimized;
            set
            {
                if (this.SetProperty(ref this.bandwidthMinimized, value))
                {
                    this.RaisePropertyChanged(nameof(this.HasChanges));
                    this.SaveCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        public bool HasChanges
        {
            get => this.DownloadDir != this.configurationService.Configuration.DownloadDir ||
                   this.Minimize != this.configurationService.Configuration.Minimize ||
                   this.Bandwidth != this.configurationService.Configuration.Bandwidth ||
                   this.BandwidthMinimized != this.configurationService.Configuration.BandwidthMinimized;
        }

        private void Save()
        {
            // TODO Request if the user really wants to save the changes.
            this.configurationService.Save(new Configuration
            {
                DownloadDir = this.DownloadDir,
                Minimize = this.Minimize,
                Bandwidth = this.Bandwidth,
                BandwidthMinimized = this.BandwidthMinimized
            });

            this.SaveCommand?.RaiseCanExecuteChanged();
        }

        private void SelectDownloadDir()
        {
            var browser = new VistaFolderBrowserDialog
            {
                SelectedPath = this.DownloadDir,
                ShowNewFolderButton = true
            };

            if (browser.ShowDialog() != null)
            {
                this.DownloadDir = browser.SelectedPath;
            }
        }
    }
}
