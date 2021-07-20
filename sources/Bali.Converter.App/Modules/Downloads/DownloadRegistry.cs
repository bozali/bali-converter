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

    using Bali.Converter.App.Services;
    using Bali.Converter.Common.Enums;
    using Bali.Converter.Common.Media;

    public class DownloadRegistry : IDownloadRegistry
    {
        private ConcurrentDictionary<Guid, DownloadJob> registry;
        private SemaphoreSlim semaphore;

        public DownloadRegistry()
        {
            this.Initialize();
        }

        public event EventHandler<DownloadEventArgs> DownloadJobAdded;
        public event EventHandler<DownloadEventArgs> DownloadJobRemoved;

        public IEnumerable<DownloadJob> Jobs
        {
            get => this.registry.Values;
        }

        public async Task<DownloadJob> Get()
        {
            await this.semaphore.WaitAsync();

            var found = this.registry.First();
            return found.Value;
        }
        
        public void Add(string url, MediaFormat format, MediaTags tags)
        {
            var job = new DownloadJob
            {
                Url = url,
                TargetFormat = format,
                Tags = tags
            };

            if (this.registry.TryAdd(job.Id, job))
            {
                using var writer = new StreamWriter(Path.Combine(IConfigurationService.ApplicationDataPath, "List.xml"));

                var serializer = new XmlSerializer(typeof(DownloadList));
                serializer.Serialize(writer, new DownloadList
                {
                    Jobs = this.registry.Select(p => p.Value).ToList()
                });

                this.semaphore.Release();

                this.OnDownloadJobAdded(new DownloadEventArgs(job));
            }
        }

        public void Remove(Guid id)
        {
            if (this.registry.TryRemove(id, out var job))
            {
                using var writer = new StreamWriter(Path.Combine(IConfigurationService.ApplicationDataPath, "List.xml"));

                var serializer = new XmlSerializer(typeof(DownloadList));
                serializer.Serialize(writer, new DownloadList
                {
                    Jobs = this.registry.Select(p => p.Value).ToList()
                });

                this.OnDownloadJobRemoved(new DownloadEventArgs(job));
            }
        }

        private void Initialize()
        {
            var file = new FileInfo(Path.Combine(IConfigurationService.ApplicationDataPath, "List.xml"));
            var serializer = new XmlSerializer(typeof(DownloadList));

            try
            {
                DownloadList list = null;

                if (!file.Exists)
                {
                    if (file.Directory is { Exists: false })
                    {
                        file.Directory.Create();
                    }

                    using var writer = file.Open(FileMode.Create, FileAccess.ReadWrite);

                    list = new DownloadList();
                    serializer.Serialize(writer, list);
                }
                else
                {
                    using var reader = file.OpenRead();
                    list = (DownloadList)serializer.Deserialize(reader);
                }

                this.registry = new ConcurrentDictionary<Guid, DownloadJob>();
                
                // ReSharper disable once PossibleNullReferenceException
                list.Jobs.ForEach(d => this.registry.TryAdd(d.Id, d));

                this.semaphore = new SemaphoreSlim(this.registry.Count);
            }
            catch (Exception e)
            {
                // TODO We need to handle corrupted list files by clearing it and creating an empty one.
            }
        }

        protected virtual void OnDownloadJobAdded(DownloadEventArgs e)
        {
            var handler = this.DownloadJobAdded;
            handler?.Invoke(this, e);
        }

        protected virtual void OnDownloadJobRemoved(DownloadEventArgs e)
        {
            var handler = this.DownloadJobRemoved;
            handler?.Invoke(this, e);
        }

    }
}
