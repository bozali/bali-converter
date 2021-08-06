namespace Bali.Converter.App.Services
{
    using System;
    using System.IO;

    using Bali.Converter.App.Modules.Settings;

    public interface IConfigurationService
    {
        static string TempPath
        {
            get => Path.Combine(Path.GetTempPath(), "BaliConverter");
        }

        static string ConfigurationPath
        {
            get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BaliConverter", "Configuration.xml");
        }

        static string ApplicationDataPath
        {
            get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BaliConverter");
        }

        Configuration Configuration { get; }

        Configuration Reload();

        void Save(Configuration configuration);
    }
}
