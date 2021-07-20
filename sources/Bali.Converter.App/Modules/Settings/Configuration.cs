namespace Bali.Converter.App.Modules.Settings
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot("Configuration")]
    public class Configuration
    {
        [XmlAttribute]
        public string DownloadDir { get; set; }
    }
}
