namespace Bali.Converter.FFmpeg
{
    using System.Threading.Tasks;

    public interface IFFmpeg
    {
        Task Convert(string path, string destination, ConversionOptions options);
    }
}
