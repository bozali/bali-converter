namespace Bali.Converter.App.Modules.Downloads
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    // TODO Maybe we don't need this class. We can just use List<DownloadJob> to serialize the data
    [Serializable]
    [XmlRoot("Queue")]
    public class DownloadQueue
    {
        public DownloadQueue()
        {
            this.Jobs = new List<DownloadJobQueueItem>();
        }

        public List<DownloadJobQueueItem> Jobs { get; set; }
    }
}
