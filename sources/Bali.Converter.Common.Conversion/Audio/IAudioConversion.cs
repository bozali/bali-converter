namespace Bali.Converter.Common.Conversion.Audio
{
    public interface IAudioConversion : IConversion
    {
        AudioConversionOptions AudioConversionOptions { get; set; }
    }
}
