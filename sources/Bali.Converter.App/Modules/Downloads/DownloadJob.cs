namespace Bali.Converter.App.Modules.Downloads
{
    using Bali.Converter.App.Events;
    using Bali.Converter.Common.Enums;
    using Bali.Converter.Common.Media;

    using LiteDB;

    public class DownloadJob
    {
        private float progress;
        private string progressText;
        private DownloadState state;

        public event DownloadStateChangedEventHandler DownloadStateChanged;
        public event DownloadProgressChangedEventHandler DownloadProgressChanged;

        public int Id { get; set; }

        public string Url { get; set; }

        public FileExtension TargetFormat { get; set; }

        public string ThumbnailPath { get; set; }

        public DownloadState State
        {
            get => this.state;
            set
            {
                this.state = value;
                this.ProgressText = this.state.ToString("G");
        
                this.OnStateChanged(this.state);
            }
        }

        public MediaTags Tags { get; set; }

        [BsonIgnore]
        public string ProgressText
        {
            get => this.progressText;
            set
            {
                this.progressText = value;
                this.OnProgressChanged(this.Progress, this.ProgressText);
            }
        }

        [BsonIgnore]
        public float Progress
        {
            get => this.progress;
            set
            {
                this.progress = value;
                this.OnProgressChanged(this.Progress, this.ProgressText);
            }
        }

        protected virtual void OnProgressChanged(float progress, string text)
        {
            this.DownloadProgressChanged?.Invoke(this, new DownloadProgressChangedEventArgs(progress, text));
        }

        protected virtual void OnStateChanged(DownloadState state)
        {
            this.DownloadStateChanged?.Invoke(this, new DownloadStateChangedEventArgs(state));
        }
    }
}
