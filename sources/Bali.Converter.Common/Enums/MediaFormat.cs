namespace Bali.Converter.Common.Enums
{
    public enum MediaFormat
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

    public static class MediaFormatExtensions
    {
        public static bool IsAudioOnly(this MediaFormat format)
        {
            return format == MediaFormat.Aac ||
                   format == MediaFormat.Flac ||
                   format == MediaFormat.MP3 ||
                   format == MediaFormat.M4a ||
                   format == MediaFormat.Opus ||
                   format == MediaFormat.Vorbis ||
                   format == MediaFormat.Wav;
        }
    }
}
