namespace Bali.Converter.App.Events
{
    using System;

    public class DownloadProgressChangedEventArgs : EventArgs
    {
        public DownloadProgressChangedEventArgs(float progress, string text)
        {
            this.Progress = progress;
            this.Text = text;
        }

        public virtual float Progress { get; set; }

        public virtual string Text { get; set; }
    }
}
