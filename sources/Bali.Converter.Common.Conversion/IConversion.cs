namespace Bali.Converter.Common.Conversion
{
    using System;
    using System.Threading.Tasks;

    using Bali.Converter.Common.Enums;

    public interface IConversion
    {
        ConversionTopology Topology { get; }

        string Extension { get; }

        Type[] SupportedTargets { get; }

        Task Convert(string path);
    }
}
