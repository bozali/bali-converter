namespace Bali.Converter.App.Modules.Downloads
{
    using System;
    using System.Xml.Serialization;

    using Bali.Converter.App.Events;
    using Bali.Converter.Common.Enums;
    using Bali.Converter.Common.Media;

    public class DownloadJob
    {
        private float progress;
        private string progressText;
        private DownloadState state;

        public DownloadJob()
        {
            this.Id = Guid.NewGuid();
        }

        public event DownloadStateChangedEventHandler DownloadStateEventChanged;
        public event DownloadProgressChangedEventHandler DownloadProgressChanged;

        [XmlAttribute]
        public Guid Id { get; set; }

        [XmlAttribute]
        public string Url { get; set; }

        [XmlAttribute]
        public FileExtension TargetFormat { get; set; }

        [XmlAttribute]
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

        [XmlElement]
        public MediaTags Tags { get; set; }

        [XmlIgnore]
        public string ProgressText
        {
            get => this.progressText;
            set
            {
                this.progressText = value;
                this.OnProgressChanged(this.Progress, this.ProgressText);
            }
        }

        [XmlIgnore]
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
            this.DownloadStateEventChanged?.Invoke(this, new DownloadStateChangedEventArgs(state));
        }
    }
}
