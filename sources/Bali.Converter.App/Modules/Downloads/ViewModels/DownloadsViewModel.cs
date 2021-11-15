namespace Bali.Converter.App.Modules.Downloads.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;

    using Bali.Converter.App.Services;
    using Bali.Converter.Common.Extensions;

    using Prism.Commands;
    using Prism.Mvvm;

    public class DownloadsViewModel : BindableBase
    {
        private readonly IDownloadRegistryService downloadRegistry;

        private ObservableCollection<DownloadJobViewModel> downloadJobs;

        public DownloadsViewModel(IDownloadRegistryService downloadRegistry)
        {
            this.downloadRegistry = downloadRegistry;

            this.DownloadJobs = new ObservableCollection<DownloadJobViewModel>();
            this.DownloadJobs.CollectionChanged += this.OnCollectionChanged;

            this.downloadRegistry.All.ForEach(j => this.DownloadJobs.Add(new DownloadJobViewModel(j)));

            this.downloadRegistry.DownloadJobAdded += this.OnDownloadJobAdded;
            this.downloadRegistry.DownloadJobRemoved += this.OnDownloadJobRemoved;
            // this.downloadRegistry.DownloadUpdated += this.OnDownloadUpdated;

            this.SelectItemCommand = new DelegateCommand<DownloadJobViewModel>(job =>
                                                                               {
                                                                                   job.IsSelected = !job.IsSelected;
                                                                                   this.DownloadJobs.Except(new[] { job }).ForEach(c => c.IsSelected = false);
                                                                               });

            this.ClearDownloadListCommand = new DelegateCommand(this.ClearDownloadList);
        }

        public DelegateCommand<DownloadJobViewModel> SelectItemCommand { get; }

        public DelegateCommand ClearDownloadListCommand { get; }

        public ObservableCollection<DownloadJobViewModel> DownloadJobs
        {
            get => this.downloadJobs;
            set => this.SetProperty(ref this.downloadJobs, value);
        }

        public bool IsDownloadsEmpty
        {
            get => !this.DownloadJobs.Any();
        }

        public bool HasDownloads
        {
            get => this.DownloadJobs.Any();
        }

        private void OnDownloadJobAdded(object sender, DownloadEventArgs e)
        {
            this.DownloadJobs.Add(new DownloadJobViewModel(e.Job));
        }

        private void OnDownloadJobRemoved(object sender, DownloadEventArgs e)
        {
            var found = this.DownloadJobs.FirstOrDefault(i => i.Id == e.Job.Id);

            if (found != null)
            {
                this.DownloadJobs.Remove(found);
            }
        }
        
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RaisePropertyChanged(nameof(this.DownloadJobs));
            this.RaisePropertyChanged(nameof(this.HasDownloads));
            this.RaisePropertyChanged(nameof(this.IsDownloadsEmpty));
        }

        private void ClearDownloadList()
        {
            if (this.IsDownloadsEmpty)
            {
                return;
            }

            var ids = this.DownloadJobs.Select(x => x.Id);

            ids.ForEach(id => this.downloadRegistry.Remove(id));
        }
    }
}
