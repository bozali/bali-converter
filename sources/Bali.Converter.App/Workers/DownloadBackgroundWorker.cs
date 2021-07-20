namespace Bali.Converter.App.Workers
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Bali.Converter.App.Modules.Downloads;
    using Bali.Converter.App.Services;
    using Bali.Converter.Common.Extensions;
    using Bali.Converter.YoutubeDl;

    public class DownloadBackgroundWorker
    {
        private readonly IConfigurationService configurationService;
        private readonly IDownloadRegistry downloadRegistry;
        private readonly IYoutubeDl youtubedl;

        public DownloadBackgroundWorker(IConfigurationService configurationService,
                                        IDownloadRegistry downloadRegistry,
                                        IYoutubeDl youtubedl)
        {
            this.configurationService = configurationService;
            this.downloadRegistry = downloadRegistry;
            this.youtubedl = youtubedl;
        }

        public async Task Process(CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                DownloadJob job = null;

                try
                {
                    job = await this.downloadRegistry.Get();

                    string downloadPathPattern = Path.Combine(IConfigurationService.TempPath, $"{job.Id:N}.%(ext)s");
                    string downloadPath = downloadPathPattern.Replace("%(ext)s", job.TargetFormat.ToString().ToLowerInvariant());
                    string destinationPath = Path.Combine(this.configurationService.Configuration.DownloadDir, job.Tags.Title + "." + job.TargetFormat.ToString().ToLowerInvariant());

                    await this.youtubedl.Download(job.Url, downloadPathPattern, job.TargetFormat);

                    File.Move(downloadPath, destinationPath);

                    this.downloadRegistry.Remove(job.Id);
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}
