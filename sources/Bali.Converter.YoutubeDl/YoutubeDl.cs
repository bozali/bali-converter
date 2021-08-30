namespace Bali.Converter.YoutubeDl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    using Bali.Converter.Common;
    using Bali.Converter.Common.Enums;
    using Bali.Converter.Common.Extensions;
    using Bali.Converter.YoutubeDl.Quality;
    using Bali.Converter.YoutubeDl.Serialization;

    using log4net;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class YoutubeDl : IYoutubeDl
    {
        private readonly ILog logger = LogManager.GetLogger(Constants.ApplicationLogger);

        private readonly string youtubedl;
        private readonly string ffmpeg;
        private readonly string temp;

        public YoutubeDl(string youtubedl, string ffmpeg, string temp)
        {
            this.youtubedl = youtubedl;
            this.ffmpeg = ffmpeg;
            this.temp = temp;
        }

        public async Task<IReadOnlyCollection<Video>> GetVideos(string url)
        {
            string pathPattern = Path.Combine(this.temp, "%(id)s.%(ext)s");

            var arguments = new List<string>();
            arguments.Add($@"--output ""{pathPattern}""");
            arguments.Add($@"""{url}""");
            arguments.Add("--write-info-json");
            arguments.Add("--skip-download");

            var fileNames = new List<string>();

            using var process = new ProcessWrapper(this.youtubedl);

            void OnProcessOnOutputDataReceived(object s, DataReceivedEventArgs e)
            {
                this.logger.Info(e.Data);

                if (string.IsNullOrEmpty(e.Data) || !e.Data.Contains(".info.json"))
                {
                    return;
                }

                var pattern = new Regex(@"[\w-]+S*\.info.json");
                var match = pattern.Match(e.Data);

                fileNames.Add(match.Value);
            }

            this.logger.Info($"Starting {this.youtubedl} with following arguments: {string.Join(' ', arguments)}");

            process.OutputDataReceived += OnProcessOnOutputDataReceived;

            await process.Execute(string.Join(' ', arguments));

            process.OutputDataReceived -= OnProcessOnOutputDataReceived;

            var result = new List<Video>();

            foreach (string fileName in fileNames)
            {
                string infoPath = Path.Combine(this.temp, fileName);
                using var reader = new StreamReader(infoPath);

                var video = JsonConvert.DeserializeObject<Video>(await reader.ReadToEndAsync(),
                                                            new IsoDateTimeConverter { DateTimeFormat = "yyyyMMdd" });
                result.Add(video);

                reader.Close();

                new FileInfo(infoPath).SafeDelete();
            }

            return result;
        }

        public async Task<Video> GetVideo(string url)
        {
            string pathPattern = Path.Combine(this.temp, "%(id)s.%(ext)s");

            var arguments = new List<string>();
            arguments.Add($@"--output ""{pathPattern}""");
            arguments.Add($@"""{url}""");
            arguments.Add("--write-info-json");
            arguments.Add("--skip-download");

            var uri = new Uri(url);
            var queries = HttpUtility.ParseQueryString(uri.Query);

            if (queries.AllKeys.Any(k => string.Equals(k, "list", StringComparison.OrdinalIgnoreCase)))
            {
                arguments.Add("--no-playlist");
            }

            string infoFileName = string.Empty;

            using var process = new ProcessWrapper(this.youtubedl);

            var handler = new DataReceivedEventHandler((s, e) =>
            {
                string extracted = YoutubeDl.ExtractFilePath(e.Data);

                if (!string.IsNullOrEmpty(extracted))
                {
                    infoFileName = extracted;
                }
            });

            process.OutputDataReceived += handler;

            await process.Execute(string.Join(' ', arguments));

            process.OutputDataReceived -= handler;

            string infoPath = Path.Combine(this.temp, infoFileName);
            using var reader = new StreamReader(infoPath);

            try
            {
                return JsonConvert.DeserializeObject<Video>(await reader.ReadToEndAsync(),
                                                            new IsoDateTimeConverter { DateTimeFormat = "yyyyMMdd" });
            }
            finally
            {
                reader.Close();

                new FileInfo(infoPath).Delete();
            }
        }

        public async Task Download(string url, DownloadOptions options, Action<float, string> progressReport = null, CancellationToken ct = default)
        {
            var arguments = new List<string>();
            arguments.Add($@"--output ""{options.Destination}""");
            arguments.Add($@"""{url}""");
            arguments.Add($@"--format best");
            arguments.Add($@"--no-playlist");

            if (options.DownloadFormat.IsAudioOnly())
            {
                arguments.Add("--extract-audio");
                arguments.Add($@"--audio-format {options.DownloadFormat.ToString("G").ToLowerInvariant()}");
            }
            else if (options.DownloadFormat != FileExtension.MP4)
            {
                arguments.Add($@"--recode-video {options.DownloadFormat.ToString("G").ToLowerInvariant()}");
            }

            if (options.DownloadFormat != FileExtension.MP4)
            {
                arguments.Add($@"--ffmpeg-location ""{this.ffmpeg}""");
            }

            if (options.DownloadBandwidth != -1)
            {
                arguments.Add($@"--http-chunk-size {options.DownloadBandwidth}");
            }

            var percentageReg = new Regex(@"([^\s]+)\%");
            var detailsReg = new Regex(@"(?<=\[download\])(.*)(?=ETA)");

            void OnOutputDataReceived(object s, DataReceivedEventArgs e)
            {
                Debug.WriteLine(e.Data);

                if (string.IsNullOrEmpty(e.Data) || !e.Data.Contains("[download]") || !e.Data.Contains("ETA"))
                {
                    return;
                }
                
                string progressStr = percentageReg.Match(e.Data).Groups[1].ToString();
                float progress = Convert.ToSingle(progressStr, CultureInfo.InvariantCulture);
                string details = detailsReg.Match(e.Data).ToString().Trim();

                var doubleSpaceReg = new Regex("[ ]{2,}", RegexOptions.None);
                details = doubleSpaceReg.Replace(details, " ");

                progressReport?.Invoke(progress, details);
            }
            
            using var process = new ProcessWrapper(this.youtubedl);

            process.OutputDataReceived += OnOutputDataReceived;
            process.ErrorDataReceived += OnOutputDataReceived;

            await process.Execute(string.Join(' ', arguments), ct);

            process.OutputDataReceived -= OnOutputDataReceived;
            process.ErrorDataReceived -= OnOutputDataReceived;
        }

        private static string ExtractFilePath(string data)
        {
            if (string.IsNullOrEmpty(data) || !data.Contains(".info.json"))
            {
                return string.Empty;
            }

            var pattern = new Regex(@"[\w-]+S*\.info.json");
            var match = pattern.Match(data);

            return match.Value;
        }
    }
}
