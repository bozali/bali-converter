namespace Bali.Converter.Common.Conversion
{
    using System;

    public interface IConversionProvider
    {
        string[] GetSupportedFormatsFor(string path);

        IConversion ProvideFor(string target);
    }
}
