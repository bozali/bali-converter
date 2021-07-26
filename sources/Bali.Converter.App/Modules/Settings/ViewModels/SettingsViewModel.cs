namespace Bali.Converter.App.Modules.Settings.ViewModels
{
    using System.Windows.Input;

    using Bali.Converter.App.Services;

    using Ookii.Dialogs.Wpf;

    using Prism.Commands;
    using Prism.Mvvm;

    public class SettingsViewModel : BindableBase
    {
        private readonly IConfigurationService configurationService;

        private string downloadDir;

        public SettingsViewModel(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;

            this.DownloadDir = this.configurationService.Configuration.DownloadDir;

            this.SaveCommand = new DelegateCommand(this.Save, () => this.HasChanges);
            this.SelectDownloadDirCommand = new DelegateCommand(this.SelectDownloadDir);

            this.RaisePropertyChanged();
        }

        public ICommand SaveCommand { get; }

        public ICommand SelectDownloadDirCommand { get; }

        public string DownloadDir
        {
            get => this.downloadDir;
            set
            {
                if (this.SetProperty(ref this.downloadDir, value))
                {
                    this.RaisePropertyChanged(nameof(this.HasChanges));
                }
            }
        }

        public bool HasChanges
        {
            get => this.DownloadDir != this.configurationService.Configuration.DownloadDir;
        }

        private void Save()
        {
            // TODO Request if the user really wants to save the changes.
            this.configurationService.Save(new Configuration
            {
                DownloadDir = this.DownloadDir
            });
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
