namespace Bali.Converter.FFmpeg
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Bali.Converter.Common;
    using Bali.Converter.FFmpeg.Filters.Audio;
    using Bali.Converter.FFmpeg.Filters.Video;

    public class FFmpeg : IFFmpeg
    {
        private readonly string ffmpegPath;

        public FFmpeg(string ffmpegPath)
        {
            this.ffmpegPath = ffmpegPath;
        }

        public async Task Convert(string path, string destination, VideoConversionOptions options)
        {
            var arguments = new List<string>();
            arguments.Add($@"-i ""{path}""");
            arguments.Add($@"-ss 00:00:""{options.From}""");
            arguments.Add($@"-t 00:00:""{options.To}""");
            arguments.Add($@"-vf ""fps={options.Fps}""");

            if (destination.EndsWith(".gif"))
            {
                arguments.Add("-loop 1");
            }

            arguments.Add(this.BuildFilterArguments(options.Filters));

            arguments.Add($@"""{destination}""");

            using var process = new ProcessWrapper(this.ffmpegPath);
            await process.Execute(string.Join(' ', arguments));
        }

        private string BuildFilterArguments(IEnumerable filters)
        {
            object[] enumerable = filters as object[] ?? filters.Cast<object>().ToArray();

            var videoFilters = enumerable.OfType<IVideoFilter>().ToArray();
            var audioFilters = enumerable.OfType<IAudioFilter>().ToArray();

            string videoFiltersArgument = string.Empty;
            string audioFiltersArgument = string.Empty;

            if (videoFilters.Any())
            {
                videoFiltersArgument += $"-vf {string.Join(' ', videoFilters.Select(f => $@"""{f.GetArgument()}"""))}";
            }

            if (audioFilters.Any())
            {
                audioFiltersArgument += $"-vf {string.Join(' ', audioFilters.Select(f => $@"""{f.GetArgument()}"""))}";
            }

            return string.Join(' ', videoFiltersArgument, audioFiltersArgument);
        }
    }
}
