namespace Bali.Converter.App.Services
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    using Bali.Converter.App.Modules.Settings;

    public class ConfigurationService : IConfigurationService
    {
        private Configuration configuration;

        public Configuration Configuration
        {
            get => this.configuration ??= this.Reload();
        }

        public Configuration Reload()
        {
            var file = new FileInfo(IConfigurationService.ConfigurationPath);

            if (!file.Exists)
            {
                this.configuration = new Configuration
                {
                    DownloadDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)),
                    FirstTime = true,
                    Minimize = true,
                    Bandwidth = -1,
                    BandwidthMinimized = -1
                };

                this.Save(this.configuration);
            }
            else
            {
                // TODO Catch exception if you deserialize
                var serializer = new XmlSerializer(typeof(Configuration));
                using var reader = new StreamReader(IConfigurationService.ConfigurationPath);

                this.configuration = (Configuration)serializer.Deserialize(reader);
            }

            return this.configuration;
        }

        public void Save(Configuration configuration)
        {
            // TODO Catch exception if you serialize
            this.configuration = configuration;

            var directory = new DirectoryInfo(Path.GetDirectoryName(IConfigurationService.ConfigurationPath)!);

            if (!directory.Exists)
            {
                directory.Create();
            }

            var serializer = new XmlSerializer(typeof(Configuration));
            
            using var stream = new StreamWriter(IConfigurationService.ConfigurationPath);
            serializer.Serialize(stream, this.configuration);
        }
    }
}
