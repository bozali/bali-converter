namespace Bali.Converter.App.Modules.Downloads.ViewModels
{
    using System;
    using System.ComponentModel;

    using Prism.Commands;
    using Prism.Mvvm;

    public class DownloadJobViewModel : BindableBase
    {
        private readonly DownloadJob job;

        private string url;
        private string headerText;
        private string progressText;
        private float progress;

        public DownloadJobViewModel(DownloadJob job)
        {
            this.job = job;
            this.job.PropertyChanged += this.OnModelPropertyChanged;

            this.HeaderText = this.job.Tags.Title;
            this.ProgressText = "Pending";
            this.Url = this.job.Url;
            this.Id = this.job.Id;
            this.Progress = 0.0f;

            this.RequestCancelCommand = new DelegateCommand(() => { });
        }

        public DelegateCommand RequestCancelCommand { get; }

        public Guid Id { get; set; }

        public string Url
        {
            get => this.url;
            set => this.SetProperty(ref this.url, value);
        }

        public string HeaderText
        {
            get => this.headerText;
            set => this.SetProperty(ref this.headerText, value);
        }

        public string ProgressText
        {
            get => this.progressText;
            set => this.SetProperty(ref this.progressText, value);
        }

        public float Progress
        {
            get => this.progress;
            set => this.SetProperty(ref this.progress, value);
        }

        private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ProgressText":
                    this.ProgressText = ((DownloadJob)sender).ProgressText;
                    break;
                case "Progress":
                    this.Progress = ((DownloadJob)sender).Progress;
                    break;
            }
        }
    }
}
