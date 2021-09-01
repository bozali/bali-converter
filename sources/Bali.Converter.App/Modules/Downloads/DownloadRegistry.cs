namespace Bali.Converter.App.Modules.Downloads
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    using Bali.Converter.App.Serialization;
    using Bali.Converter.App.Services;
    using Bali.Converter.Common;
    using Bali.Converter.Common.Extensions;

    using log4net;

    public class DownloadRegistry : IDownloadRegistry
    {
        private readonly ILog logger;

        private ConcurrentDictionary<Guid, DownloadJobQueueItem> registry;
        private SemaphoreSlim semaphore;

        public DownloadRegistry()
        {
            this.logger = LogManager.GetLogger(Constants.DownloadServiceLogger);

            this.Initialize();
        }

        public event EventHandler<DownloadEventArgs> DownloadJobAdded;
        public event EventHandler<DownloadEventArgs> DownloadJobRemoved;

        public IEnumerable<DownloadJobQueueItem> Jobs
        {
            get => this.registry.Values;
        }

        private string QueuePath
        {
            get => Path.Combine(IConfigurationService.ApplicationDataPath, "Queue.xml");
        }

        public async Task<DownloadJobQueueItem> Get()
        {
            await this.semaphore.WaitAsync();

            var found = this.registry
                            .Where(j => j.Value.State == DownloadState.Downloading || j.Value.State == DownloadState.Pending)
                            .OrderBy(j => j.Value.Index)
                            .FirstOrDefault();

            return found.Value;
        }

        public void Complete(DownloadJob job)
        {
            if (this.registry.TryGetValue(job.Id, out var item))
            {
                item.State = DownloadState.Completed;

                this.UpdateDownloadQueue();
            }
        }

        public void Add(DownloadJob job)
        {
            this.logger.Info($"Registering {job.Id:B}");

            // Create a new job details
            var file = new FileInfo(Path.Combine(IConfigurationService.ApplicationDataPath, "Details", job.Id.ToString("B") + ".xml"));

            if (file.Directory is { Exists: false })
            {
                file.Directory.Create();
            }

            var item = new DownloadJobQueueItem
            {
                DetailsPath = file.FullName,
                Index = this.registry.Count,
                Id = job.Id,
                Details = job
            };

            if (!this.registry.TryAdd(item.Id, item))
            {
                return;
            }

            // Creating a new details file of the job.
            ImmediateXmlSerializer.Serialize(file.FullName, job);

            this.OnDownloadJobAdded(new DownloadEventArgs(item));
        }

        public void Remove(Guid id)
        {
            // TODO Also remove the details file
            if (this.registry.TryRemove(id, out var job))
            {
                this.OnDownloadJobRemoved(new DownloadEventArgs(job));
            }
        }

        public void Cancel(Guid id)
        {
            if (this.registry.TryGetValue(id, out var item))
            {
                // Changing the reference of the item to canceled and saving it.
                item.State = DownloadState.Canceled;

                this.UpdateDownloadQueue();
            }
        }

        private void Initialize()
        {
            var file = new FileInfo(Path.Combine(IConfigurationService.ApplicationDataPath, "Queue.xml"));

            try
            {
                var queue = new DownloadQueue();

                if (!file.Exists)
                {
                    file.CreateDirectory();
                    ImmediateXmlSerializer.Serialize(file.FullName, queue);
                }
                else
                {
                    queue = ImmediateXmlSerializer.Deserialize<DownloadQueue>(file.FullName);
                }

                this.registry = new ConcurrentDictionary<Guid, DownloadJobQueueItem>();

                // ReSharper disable once PossibleNullReferenceException
                foreach (var item in queue.Jobs)
                {
                    item.Details = ImmediateXmlSerializer.Deserialize<DownloadJob>(item.DetailsPath);
                    this.registry.TryAdd(item.Id, item);
                }

                this.semaphore = new SemaphoreSlim(this.registry.Count);
            }
            catch (Exception e)
            {
                // TODO We need to handle corrupted list files by clearing it and creating an empty one.
            }
        }

        private void UpdateDownloadQueue()
        {
            ImmediateXmlSerializer.Serialize(this.QueuePath, new DownloadQueue
            {
                Jobs = this.registry.Select(p => p.Value).ToList()
            });
        }

        protected virtual void OnDownloadJobAdded(DownloadEventArgs e)
        {
            this.logger.Info($"Download job added: {e.Job.Id}");

            this.UpdateDownloadQueue();

            this.semaphore.Release();

            var handler = this.DownloadJobAdded;
            handler?.Invoke(this, e);
        }

        protected virtual void OnDownloadJobRemoved(DownloadEventArgs e)
        {
            this.logger.Info($"Download job removed: {e.Job.Id}");

            this.UpdateDownloadQueue();

            var handler = this.DownloadJobRemoved;
            handler?.Invoke(this, e);
        }
    }
}
