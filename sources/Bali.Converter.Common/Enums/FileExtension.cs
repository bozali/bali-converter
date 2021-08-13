namespace Bali.Converter.Common.Enums
{
    public enum FileExtension
    {
        MP3,

        MP4,

        Wav,

        M4a,

        Opus,

        Vorbis,

        Aac,

        Flac,

        Flv,

        Ogg,

        Webm,

        Mkv,

        Avi
    }

    public static class FileTypeExtensions
    {
        public static bool IsAudioOnly(this FileExtension type)
        {
            return type == FileExtension.Aac ||
                   type == FileExtension.Flac ||
                   type == FileExtension.MP3 ||
                   type == FileExtension.M4a ||
                   type == FileExtension.Opus ||
                   type == FileExtension.Vorbis ||
                   type == FileExtension.Wav;
        }
    }
}
