namespace Bali.Converter.FFmpeg
{
    using System.Threading.Tasks;

    using Bali.Converter.FFmpeg.Models;

    public interface IFFmpeg
    {
        Task Convert(string path, string destination, VideoConversionOptions options);
    }
}
