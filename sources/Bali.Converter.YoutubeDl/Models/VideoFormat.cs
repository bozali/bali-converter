namespace Bali.Converter.YoutubeDl.Models
{
    using System;

    using Newtonsoft.Json;

    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class VideoFormat
    {
        [JsonProperty("format_id")]
        public int FormatId { get; set; }

        [JsonProperty("fps")]
        public int? Fps { get; set; }

        [JsonProperty("width")]
        public int? Width { get; set; }

        [JsonProperty("height")]
        public int? Height { get; set; }

        /// <summary>
        /// Average audio bit rate in KBit/s
        /// </summary>
        [JsonProperty("abr")]
        public float AverageAudioBitRate { get; set; }

        /// <summary>
        /// Average video bit rate in KBit/s
        /// </summary>
        [JsonProperty("vbr")]
        public float AverageVideoBitRate { get; set; }

        /// <summary>
        /// Audio sampling rate in Hertz.
        /// </summary>
        [JsonProperty("asr")]
        public int AudioSamplingRate
}
