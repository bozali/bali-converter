﻿namespace Bali.Converter.Common.Conversion.Video
{
    public interface IVideoConversion : IConversion
    {
        VideoConversionOptions VideoConversionOptions { get; set; }
    }
}
