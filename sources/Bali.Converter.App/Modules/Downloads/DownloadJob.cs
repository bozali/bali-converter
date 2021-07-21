namespace Bali.Converter.App.Modules.Downloads
{
    using System;
    using System.Xml.Serialization;

    using Bali.Converter.Common.Enums;
    using Bali.Converter.Common.Media;

    public class DownloadJob
    {
        public DownloadJob()
        {
            this.Id = Guid.NewGuid();
        }

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
    }
}
