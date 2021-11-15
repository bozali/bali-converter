namespace Bali.Converter.App.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bali.Converter.App.Modules.Downloads;
    using Bali.Converter.Common.Enums;

    public interface IDownloadRegistryService
    {
        public event EventHandler<DownloadEventArgs> DownloadJobAdded;
        public event EventHandler<DownloadEventArgs> DownloadJobRemoved;

        List<DownloadJob> All { get; }

        Task<DownloadJob> GetDownload();

        Task<DownloadJob> GetFetch();

        void AddFetch(string url, FileExtension targetFormat);

        void AddDownload(DownloadJob job);

        void DownloadFetched(DownloadJob job);

        void Remove(int id);

        void Complete(int id);
    }
}
