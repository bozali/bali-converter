namespace Bali.Converter.App.Modules.Downloads
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bali.Converter.Common.Enums;
    using Bali.Converter.Common.Media;

    public interface IDownloadRegistry
    {
        event EventHandler<DownloadEventArgs> DownloadJobAdded;
        event EventHandler<DownloadEventArgs> DownloadJobRemoved;

        IEnumerable<DownloadJob> Jobs { get; }

        Task<DownloadJob> Get();

        void Add(string url, MediaFormat format, MediaTags tags);

        void Remove(Guid id);
    }
}
