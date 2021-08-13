﻿namespace Bali.Converter.Common.Enums
{
    using System;

    /// <summary>
    /// Depending on which topology is supported we can use a converter, its functionality and options.
    /// </summary>
    [Flags]
    public enum ConversionTopology
    {
        None = 0,

        Audio = 1 << 0,

        Document = 1 << 1,

        Video = 1 << 2,

        Image = 1 << 3
    }

    public static class ConversionTopologyExtensions
    {
    }
}
