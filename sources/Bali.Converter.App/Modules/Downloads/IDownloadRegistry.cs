namespace Bali.Converter.App.Modules.Downloads
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Bali.Converter.Common.Enums;

    public interface IDownloadRegistry
    {
        event EventHandler<DownloadEventArgs> DownloadJobAdded;
        event EventHandler<DownloadEventArgs> DownloadJobRemoved;

        IEnumerable<DownloadJob> Jobs { get; }

        Task<DownloadJob> Get();

        void Add(string url, MediaFormat format);
    }
}
