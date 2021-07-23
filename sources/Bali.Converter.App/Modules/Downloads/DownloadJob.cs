namespace Bali.Converter.App.Modules.Downloads
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Xml.Serialization;

    using Bali.Converter.App.Annotations;
    using Bali.Converter.Common.Enums;
    using Bali.Converter.Common.Media;

    public class DownloadJob : INotifyPropertyChanged
    {
        private float progress;
        private string progressText;

        public DownloadJob()
        {
            this.Id = Guid.NewGuid();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute]
        public Guid Id { get; set; }

        [XmlAttribute]
        public string Url { get; set; }

        [XmlAttribute]
        public MediaFormat TargetFormat { get; set; }

        [XmlAttribute]
        public string ThumbnailPath { get; set; }

        [XmlElement]
        public MediaTags Tags { get; set; }

        [XmlIgnore]
        public string ProgressText
        {
            get => this.progressText;
            set
            {
                this.progressText = value;
                this.OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public float Progress
        {
            get => this.progress;
            set
            {
                this.progress = value;
                this.OnPropertyChanged();
            }
        }
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
