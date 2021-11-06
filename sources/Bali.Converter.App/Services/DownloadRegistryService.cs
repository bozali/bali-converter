namespace Bali.Converter.App.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Bali.Converter.App.Modules.Downloads;

    using LiteDB;

    using log4net;

    public class DownloadRegistryService : IDownloadRegistryService
    {
        private readonly ILiteCollection<DownloadJob> collection;
        private readonly ILog logger;
        private readonly SemaphoreSlim semaphore;

        public DownloadRegistryService(ILiteDatabase database)
        {
            this.logger = LogManager.GetLogger(typeof(DownloadRegistryService));

            this.collection = database.GetCollection<DownloadJob>();
            this.semaphore = new SemaphoreSlim(this.collection.Query().Where(x => x.State == DownloadState.Downloading || x.State == DownloadState.Pending).Count());

            this.All = this.collection.Query().ToList();
        }

        public event EventHandler<DownloadEventArgs> DownloadJobAdded;
        public event EventHandler<DownloadEventArgs> DownloadJobRemoved;

        public List<DownloadJob> All { get; private set; }

        public async Task<DownloadJob> Get()
        {
            await this.semaphore.WaitAsync();

            // TODO Maybe order by index or something
            return this.All.FirstOrDefault();
        }

        public void Add(DownloadJob job)
        {
            this.logger.Info($"Registering [{job.Id}]");
            this.collection.Insert(job);
            this.All.Add(job);

            this.OnDownloadJobAdded(new DownloadEventArgs(job));
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

            this.semaphore.Release();

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
