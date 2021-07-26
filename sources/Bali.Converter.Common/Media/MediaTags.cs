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
        public string Comment { get; set; }

        [XmlAttribute]
        public string Copyright { get; set; }

        [XmlAttribute]
        public int Year { get; set; }

        [XmlAttribute("AlbumArtists")]
        public string AlbumArtists { get; set; }

        [XmlAttribute("Genres")]
        public string Genres { get; set; }

        [XmlAttribute("Performers")]
        public string Performers { get; set; }

        [XmlAttribute("Composers")]
        public string Composers { get; set; }
    }
}
