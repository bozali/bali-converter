namespace Bali.Converter.App.Services
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography.Pkcs;
    using System.Threading;
    using System.Threading.Tasks;

    using Bali.Converter.App.Modules.Downloads;

    using LiteDB;

    using log4net;

    public class DownloadRegistryService : IDownloadRegistryService
    {
        private readonly ILiteCollection<DownloadJob> collection;
        private readonly ILiteDatabase database;
        private readonly ILog logger;
        private readonly SemaphoreSlim semaphore;

        public DownloadRegistryService(ILiteDatabase database)
        {
            this.logger = LogManager.GetLogger(typeof(DownloadRegistryService));
            this.database = database;

            this.collection = this.database.GetCollection<DownloadJob>();

            this.semaphore = new SemaphoreSlim(this.collection.Query().Where(x => x.State == DownloadState.Downloading || x.State == DownloadState.Pending).Count());
        }

        public event EventHandler<DownloadEventArgs> DownloadJobAdded;
        public event EventHandler<DownloadEventArgs> DownloadJobRemoved;

        public IReadOnlyCollection<DownloadJob> All
        {
            get => this.collection.Query().ToArray();
        }

        public async Task<DownloadJob> Get()
        {
            await this.semaphore.WaitAsync();

            // TODO Maybe order by index or something
            return this.collection
                       .Query()
                       .FirstOrDefault();
        }

        public void Add(DownloadJob job)
        {
            this.logger.Info($"Registering [{job.Id}]");
            this.collection.Insert(job);

            this.OnDownloadJobAdded(new DownloadEventArgs(job));
        }

        public void Remove(int id)
        {
            this.logger.Info($"Removing [{id}]");
            this.collection.Delete(id);

            // this.OnDownloadJobRemoved(new DownloadEventArgs());
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
