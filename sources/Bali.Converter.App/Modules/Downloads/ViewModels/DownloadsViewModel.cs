﻿namespace Bali.Converter.App.Modules.Downloads.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using Bali.Converter.Common.Enums;
    using Bali.Converter.Common.Extensions;
    using Bali.Converter.Common.Media;
    using Prism.Mvvm;

    public class DownloadsViewModel : BindableBase
    {
        private readonly IDownloadRegistry downloadRegistry;

        private ObservableCollection<DownloadJobViewModel> downloadJobs;

        public DownloadsViewModel(IDownloadRegistry downloadRegistry)
        {
            this.downloadRegistry = downloadRegistry;

            this.DownloadJobs = new ObservableCollection<DownloadJobViewModel>();
            this.DownloadJobs.CollectionChanged += this.OnCollectionChanged;

            this.downloadRegistry.Jobs.ForEach(j => this.DownloadJobs.Add(new DownloadJobViewModel(j)));
            
            this.downloadRegistry.DownloadJobAdded += this.OnDownloadJobAdded;
            this.downloadRegistry.DownloadJobRemoved += this.OnDownloadJobRemoved;
        }
        
        public ObservableCollection<DownloadJobViewModel> DownloadJobs
        {
            get => this.downloadJobs;
            set => this.SetProperty(ref this.downloadJobs, value);
        }

        public bool IsDownloadsEmpty
        {
            get => !this.DownloadJobs.Any();
        }

        private void OnDownloadJobAdded(object sender, DownloadEventArgs e)
        {
            this.DownloadJobs.Add(new DownloadJobViewModel(e.Job));
        }

        private void OnDownloadJobRemoved(object sender, DownloadEventArgs e)
        {
            var found = this.DownloadJobs.FirstOrDefault(i => i.Id == e.Job.Id);
            this.DownloadJobs.Remove(found);
        }
        
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RaisePropertyChanged(nameof(this.DownloadJobs));
            this.RaisePropertyChanged(nameof(this.IsDownloadsEmpty));
        }
    }
}
