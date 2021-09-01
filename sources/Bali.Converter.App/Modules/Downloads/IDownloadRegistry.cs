namespace Bali.Converter.App.Modules.Downloads
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDownloadRegistry
    {
        event EventHandler<DownloadEventArgs> DownloadJobAdded;
        event EventHandler<DownloadEventArgs> DownloadJobRemoved;

        IEnumerable<DownloadJobQueueItem> Jobs { get; }

        Task<DownloadJobQueueItem> Get();

        void Complete(DownloadJob job);
        
        void Add(DownloadJob job);

        void Remove(Guid id);

        void Cancel(Guid id);
    }
}
