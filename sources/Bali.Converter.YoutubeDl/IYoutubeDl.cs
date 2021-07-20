namespace Bali.Converter.YoutubeDl
{
    using System.Threading.Tasks;

    using Bali.Converter.YoutubeDl.Models;

    public interface IYoutubeDl
    {
        Task<Video> GetVideo(string url);
    }
}
