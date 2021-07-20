﻿namespace Bali.Converter.App.Modules.Downloads
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    // TODO Maybe we don't need this class. We can just use List<DownloadJob> to serialize the data
    [Serializable]
    [XmlRoot("List")]
    public class DownloadList
    {
        public DownloadList()
        {
            this.Jobs = new List<DownloadJob>();
        }

        public List<DownloadJob> Jobs { get; set; }
    }
}
