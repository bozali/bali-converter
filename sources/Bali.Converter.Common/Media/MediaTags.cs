namespace Bali.Converter.Common.Media
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    public class MediaTags
    {
        [XmlAttribute]
        public string Title { get; set; }

        [XmlAttribute]
        public string Artist { get; set; }

        [XmlAttribute]
        public string Album { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        [XmlAttribute]
        public string Copyright { get; set; }

        [XmlAttribute]
        public int Year { get; set; }

        [XmlElement("Genres")]
        public string[] Genres { get; set; }

        [XmlElement("Performers")]
        public string[] Performers { get; set; }
    }
}
