namespace Bali.Converter.App.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bali.Converter.App.Modules.Downloads;

    public interface IDownloadRegistryService
    {
        public event EventHandler<DownloadEventArgs> DownloadJobAdded;
        public event EventHandler<DownloadEventArgs> DownloadJobRemoved;

        List<DownloadJob> All { get; }

        Task<DownloadJob> Get();

        void Add(DownloadJob job);

        void Remove(int id);
    }
}
