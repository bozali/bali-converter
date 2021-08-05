namespace Bali.Converter.FFmpeg
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Bali.Converter.Common;
    using Bali.Converter.FFmpeg.Models;

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

            arguments.Add($@"""{destination}""");

            using var process = new ProcessWrapper(this.ffmpegPath);
            await process.Execute(string.Join(' ', arguments));
        }
    }
}
