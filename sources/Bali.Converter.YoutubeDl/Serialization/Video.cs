﻿namespace Bali.Converter.YoutubeDl.Serialization
{
    using System;

    using Newtonsoft.Json;

    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class Video
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("channel_url")]
        public string ChannelUrl { get; set; }

        [JsonProperty("thumbnail")]
        public string ThumbnailUrl { get; set; }

        [JsonProperty("average_rating")]
        public float AverageRating { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("webpage_url")]
        public string Url { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("upload_date")]
        public DateTime UploadDate { get; set; }

        [JsonProperty("formats")]
        public VideoFormat[] Formats { get; set; }
    }
}
