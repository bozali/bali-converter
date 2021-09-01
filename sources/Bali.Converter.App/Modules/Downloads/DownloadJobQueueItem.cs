namespace Bali.Converter.App.Modules.Downloads
{
    using System;
    using System.Printing;
    using System.Xml.Serialization;
    using Bali.Converter.App.Events;


    [Serializable]
    public class DownloadJobQueueItem
    {
        private DownloadState state;

        public event DownloadStateChangedEventHandler DownloadStateChanged;

        [XmlAttribute]
        public Guid Id { get; set; }

        [XmlAttribute]
        public int Index { get; set; }

        [XmlAttribute]
        public string DetailsPath { get; set; }

        [XmlAttribute]
        public DownloadState State
        {
            get => this.state;
            set
            {
                if (this.state != value)
                {
                    this.state = value;
                    this.OnStateChanged(this.state);

                    if (this.Details != null)
                    {
                        this.Details.ProgressText = this.state.ToString("G");
                    }
                }
            }
        }

        [XmlIgnore]
        public DownloadJob Details { get; set; }

        private void OnStateChanged(DownloadState state)
        {
            this.DownloadStateChanged?.Invoke(this, new DownloadStateChangedEventArgs(state));
        }
    }
}
