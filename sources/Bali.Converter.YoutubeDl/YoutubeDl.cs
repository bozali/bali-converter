namespace Bali.Converter.YoutubeDl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web;

    using Bali.Converter.Common;
    using Bali.Converter.Common.Enums;
    using Bali.Converter.YoutubeDl.Models;

    using Newtonsoft.Json;

    public class YoutubeDl : IYoutubeDl
    {
        private readonly string youtubedl;
        private readonly string ffmpeg;
        private readonly string temp;

        public YoutubeDl(string youtubedl, string ffmpeg, string temp)
        {
            this.youtubedl = youtubedl;
            this.ffmpeg = ffmpeg;
            this.temp = temp;
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
                return JsonConvert.DeserializeObject<Video>(await reader.ReadToEndAsync());
            }
            finally
            {
                reader.Close();

                new FileInfo(infoPath).Delete();
            }
            
        }

        public async Task Download(string url, string path, MediaFormat format)
        {
            var arguments = new List<string>();
            arguments.Add($@"--output ""{path}""");
            arguments.Add($@"""{url}""");
            arguments.Add($@"--format best");
            arguments.Add($@"--no-playlist");

            if (format != MediaFormat.MP4)
            {
                arguments.Add($@"--recode-video {format.ToString("G").ToLowerInvariant()}");
                arguments.Add($@"--ffmpeg-location ""{this.ffmpeg}""");
            }

            var process = new ProcessWrapper(this.youtubedl);
            await process.Execute(string.Join(' ', arguments));
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
