namespace Bali.Converter.App.Modules.Downloads
{
    using System;

    public class DownloadEventArgs : EventArgs
    {
        public DownloadEventArgs(DownloadJob job)
        {
            this.Job = job;
        }

        public DownloadJob Job { get; set; }
    }
}
