namespace Bali.Converter.App.Modules.Downloads.ViewModels
{
    using System;

    using Bali.Converter.App.Events;
    using Bali.Converter.Common.Media;

    using Prism.Commands;
    using Prism.Mvvm;

    public class DownloadJobViewModel : BindableBase
    {
        private readonly DownloadJobQueueItem job;

        //private MediaTagsViewModel tags;

        private string url;
        private string headerText;
        private string progressText;
        private float progress;
        private bool isSelected;

        public DownloadJobViewModel(IDownloadRegistry downloadRegistry, DownloadJobQueueItem job)
        {
            this.job = job;
            this.job.Details.DownloadProgressChanged += this.OnDownloadProgressChanged;

            this.ProgressText = this.job.State.ToString("G");
            this.HeaderText = this.job.Details.Tags.Title;
            this.Progress = job.Details.Progress;
            this.Url = this.job.Details.Url;
            this.Id = this.job.Id;

            this.RequestCancelCommand = new DelegateCommand(() => downloadRegistry.Cancel(this.job.Id));
            // this.ResumeCommand = new DelegateCommand(() => this.job.State = DownloadState.Pending);
        }

        public DelegateCommand RequestCancelCommand { get; }

        public DelegateCommand ResumeCommand { get; }

        public Guid Id { get; set; }

        public bool IsSelected
        {
            get => this.isSelected;
            set => this.SetProperty(ref this.isSelected, value);
        }

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

        public MediaTags Tags
        {
            get => this.job.Details.Tags;
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.Progress = e.Progress;
            this.ProgressText = e.Text;
        }
    }
}
