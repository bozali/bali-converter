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
    }
}
