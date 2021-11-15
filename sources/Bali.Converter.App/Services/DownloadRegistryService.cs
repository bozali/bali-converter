namespace Bali.Converter.App.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Bali.Converter.App.Modules.Downloads;
    using Bali.Converter.Common.Enums;
    using LiteDB;

    using log4net;

    public class DownloadRegistryService : IDownloadRegistryService
    {
        private readonly ILiteCollection<DownloadJob> collection;
        private readonly ILog logger;
        private readonly SemaphoreSlim downloadSemaphore;
        private readonly SemaphoreSlim fetchSemaphore;

        public DownloadRegistryService(ILiteDatabase database)
        {
            this.logger = LogManager.GetLogger(typeof(DownloadRegistryService));

            this.collection = database.GetCollection<DownloadJob>();
            this.All = this.collection.Query().ToList();

            this.downloadSemaphore = new SemaphoreSlim(this.collection.Query().Where(x => x.State == DownloadState.Downloading || x.State == DownloadState.Pending).Count());
            this.fetchSemaphore = new SemaphoreSlim(this.collection.Query().Where(x => x.State == DownloadState.Fetching).Count());
        }

        public event EventHandler<DownloadEventArgs> DownloadJobAdded;
        public event EventHandler<DownloadEventArgs> DownloadJobRemoved;

        public List<DownloadJob> All { get; private set; }

        public async Task<DownloadJob> GetDownload()
        {
            await this.downloadSemaphore.WaitAsync();

            // TODO Maybe order by index or something
            return this.All.FirstOrDefault(x => x.State == DownloadState.Downloading || x.State == DownloadState.Pending);
        }

        public async Task<DownloadJob> GetFetch()
        {
            await this.fetchSemaphore.WaitAsync();

            return this.All.FirstOrDefault(x => x.State == DownloadState.Fetching);
        }

        public void AddFetch(string url, FileExtension targetFormat)
        {
            var job = new DownloadJob
            {
                Url = url,
                TargetFormat = targetFormat,
                State = DownloadState.Fetching
            };

            this.logger.Info($"Registering [{job.Id}]");

            this.collection.Insert(job);
            this.All.Add(job);

            this.fetchSemaphore.Release();
            this.OnDownloadJobAdded(new DownloadEventArgs(job));
        }

        public void AddDownload(DownloadJob job)
        {
            this.logger.Info($"Registering [{job.Id}]");

            this.collection.Insert(job);
            this.All.Add(job);

            this.downloadSemaphore.Release();
            this.OnDownloadJobAdded(new DownloadEventArgs(job));
        }

        public void DownloadFetched(DownloadJob job)
        {
            job.State = DownloadState.Pending;
            this.collection.Update(job);

            this.downloadSemaphore.Release();
        }

        public void Remove(int id)
        {
            this.logger.Info($"Removing [{id}]");
            this.collection.Delete(id);

            var found = this.All.FirstOrDefault(j => j.Id == id);
            this.All.Remove(found);

            this.OnDownloadJobRemoved(new DownloadEventArgs(found));
        }

        public void Complete(int id)
        {
            this.logger.Info($"Completing [{id}]");

            var found = this.All.FirstOrDefault(j => j.Id == id);

            if (found != null)
            {
                found.State = DownloadState.Completed;

                this.collection.Update(found);
            }
        }

        protected virtual void OnDownloadJobAdded(DownloadEventArgs e)
        {
            this.logger.Info($"Download job added: {e.Job.Id}");

            var handler = this.DownloadJobAdded;
            handler?.Invoke(this, e);
        }

        protected virtual void OnDownloadJobRemoved(DownloadEventArgs e)
        {
            this.logger.Info($"Download job removed: {e.Job.Id}");

            var handler = this.DownloadJobRemoved;
            handler?.Invoke(this, e);
        }
    }
}
