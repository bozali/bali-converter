namespace Bali.Converter.App.Modules.Downloads
{
    using System;

    public class DownloadEventArgs : EventArgs
    {
        public DownloadEventArgs(DownloadJobQueueItem job)
        {
            this.Job = job;
        }

        public DownloadJobQueueItem Job { get; set; }
    }
}
