namespace Bali.Converter.App.Events
{
    using System;

    using Bali.Converter.App.Modules.Downloads;

    public class DownloadStateChangedEventArgs : EventArgs
    {
        public DownloadStateChangedEventArgs(DownloadState state)
        {
            this.State = state;
        }

        public virtual DownloadState State { get; set; }
    }
}
