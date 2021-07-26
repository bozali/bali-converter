﻿namespace Bali.Converter.App.Modules.Downloads.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;

    using Bali.Converter.Common.Enums;
    using Bali.Converter.Common.Extensions;
    using Bali.Converter.Common.Media;

    using Prism.Commands;
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

            this.DownloadJobs.Add(new DownloadJobViewModel(new DownloadJob
            {
                Url = "http://www.youtube.de",
                Progress = 44.0f,
                ProgressText = "Some progress text",
                Tags = new MediaTags
                {
                    Title = "Sometitle",
                    AlbumArtists = "My Album",
                    Year = 2001,
                    Copyright = "Public"
                },
                TargetFormat = MediaFormat.MP4
            }));

            this.SelectItemCommand = new DelegateCommand<DownloadJobViewModel>(job =>
                                                                               {
                                                                                   job.IsSelected = !job.IsSelected;
                                                                                   this.DownloadJobs.Except(new[] { job }).ForEach(c => c.IsSelected = false);
                                                                               });
        }

        public DelegateCommand<DownloadJobViewModel> SelectItemCommand { get; }

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
